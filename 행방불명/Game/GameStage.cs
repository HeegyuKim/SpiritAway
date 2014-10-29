using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 행방불명.Framework;
using 행방불명.Framework.UI;
using 행방불명.Game.Map;
using 행방불명.Game.Process;
using SharpDX;
using SharpDX.Direct2D1;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json;
using Microsoft.Speech.Recognition;
using IrrKlang;


namespace 행방불명.Game
{
	public class GameObject
	{
		public float x, y;
		public Bitmap bitmap;
		public string key;

		public GameObject(float x, float y, string key, Bitmap bitmap)
		{
			this.x = x;
			this.y = y;
			this.key = key;
			this.bitmap = bitmap;
		}
	}

	public class GameStage : Stage
	{
		Program app;
		Color4 bg;
		Stage next;
		string mapPath;
		GameStageOptions options;

		public GameStage(
			Program app,
			string map,
			Stage next
			)
		{
			this.app = app;
			this.bg = new Color4(0, 0, 0, 1);
			this.next = next;
			this.mapPath = map;
			this.options = new GameStageOptions()
			{
				HasTimeUI = true,
				HasMapUI = true,
				HasCountUI = true,
				Player = null
			};
		}

		public GameStage(
			Program app,
			string map,
			Stage next,
			GameStageOptions options
			)
		{
			this.app = app;
			this.bg = new Color4(0, 0, 0, 1);
			this.next = next;
			this.mapPath = map;
			this.options = options;
		}

		public ScriptView ScriptView { get { return scriptView; } }
		public Player Player { get { return player;  } }
		public MapData Map { get { return map; } }
		public Program App { get { return app; } }
		public List<IProcess> Processes { get { return processes; } }
		public List<GameObject> GameObjects { get { return gameObjects; } }

		Container container;
		ScriptView scriptView;
		GameView gameView;
		TimeView timeView;
		CountView countView;
		SettingButtons exitBtn;
		Bitmap playerBitmap, ping;
		float playerAngle = 0, pingDelta = 0;
		readonly float SoundDistanceRate = 100.0f;

		public void Start()
		{
			Console.WriteLine(mapPath + " Game Stage started.");

			playerBitmap = app.Media.BitmapDic["character"];
			ping = app.Media.BitmapDic["ping"];
			gameObjects = new List<GameObject>();

			center = new Vector2(app.Width / 2, app.Height / 2);
			voice = app.VoiceControl;
			processBuilder = new ProcessBuilder(app, this);

			LoadMap(mapPath);
			InitUI(options);
		}

		private void InitUI(GameStageOptions options)
		{
			player.HasHammer = true;
			//player.HasKey = true;
			//player.NumMedicalKits = 5;

			container = new Container(app);
			gameView = new GameView(app);
			scriptView = new ScriptView(app);
			exitBtn = new SettingButtons(app);

			container.Views.Add(gameView);
			container.Views.Add(exitBtn);
			container.Views.Add(scriptView);

			if (options.HasCountUI)
			{
				countView = new CountView(app, player);
				container.Views.Add(countView);
			}
			if (options.HasMapUI)
			{

			}
			if (options.HasTimeUI)
			{
				timeView = new TimeView(app);
				container.Views.Add(timeView);
			}

			
			
		}



		MapData map;
		List<GameObject> gameObjects;


		private void SetupSurvivor(Survivor surv)
		{
			Waypoint way = map.GetWaypoint(surv.Id);
			if (way == null)
			{
				Console.WriteLine("알 수 없는 생존자의 연결 ID입니다. " + surv.Id);
			}

			Console.WriteLine(
				"{0} ({1}, {2}): {3}",
				surv.Id, surv.X, surv.Y, surv.Relief
				);
			if (surv.Relief == null)
			{
				return;
			}

			ISoundEngine engine = app.Sound;
			var source = engine.GetSoundSource(surv.Relief);
			if (source == null)
			{
				Console.WriteLine(surv.Relief + " 생존자의 말을 찾을 수 없어요.");
				return;
			}
			surv.SX = surv.X / SoundDistanceRate;
			surv.SY = surv.Y / SoundDistanceRate;
			surv.LoopCycle = source.PlayLength / 1000.0f + 10;
		}

		private void LoadMap(string path)
		{
			string json = File.ReadAllText(path);
			map = JsonConvert.DeserializeObject<MapData>(json, new JsonSerializerSettings()
			{
				Culture = new System.Globalization.CultureInfo("ko-kr")
			});
			//map.ConvertEncoding();


			//
			//
			// Choices loading...

			Choices choices = new Choices();

			foreach (var waypoints in map.Waypoints)
			{
				if (waypoints.Scripts != null)
					foreach (var script in waypoints.Scripts)
					{
						var text = script.PlayerText;
						if(text != null && text.Length > 0)
							choices.Add(script.PlayerText);
					}
				if (waypoints.Links != null)
					foreach (var link in waypoints.Links)
						choices.Add(link.Name);
			}

			if (map.Survivors != null)
			{
				foreach (var surv in map.Survivors)
					SetupSurvivor(surv);
			}

			if (map.Choices != null)
				choices.Add(map.Choices.ToArray());

			GrammarBuilder builder = new GrammarBuilder();
			builder.Append(choices);
			Grammar grammar = new Grammar(builder);
			voice.Engine.LoadGrammar(grammar);

			//
			// set starting point
			currWaypoint = map.GetWaypoint(map.PlayerPosition);
			if (options.Player == null)
				player = new Player(currWaypoint);
			else
			{
				player = options.Player;
				player.Change(currWaypoint);
			}
		}

		bool ended = false;
		VoiceControl voice;
		Player player;
		Waypoint currWaypoint;
		List<IProcess> processes = new List<IProcess>();
		ProcessBuilder processBuilder;
		Vector2 currPlayerPos, playerNormal;
		Vector2 listenerPos = new Vector2();
		float checkingDelta = 0;

		private void Process(float delta)
		{
			if (processes.Count == 0) return;

			var p = processes.First();
			p.Update(delta);

			// 다 처리됬으면 없애기
			if (p.IsEnded())
			{
				processes.RemoveAt(0);
				p.End();
				if (processes.Count > 0)
					processes.First().Start();
			}
		}

		public void Update(float delta)
		{
			playerAngle += delta * 3.141592f / 4;
			pingDelta += delta;
			if (pingDelta > 2)
				pingDelta = 0;
			if (playerAngle > 6.283)
				playerAngle = 0;

			UpdateReliefSound(delta);



			container.Update(delta);
			gameView.Center = player.CurrentPosition;

			// 플레이어 상태에 따른 처리
			switch(player.State)
			{
				case PlayerState.Arrived:
					processes.Clear();
					processBuilder.AddProcess(processes, player.MoveTo);
					player.State = PlayerState.Processing;
					break;

				case PlayerState.Processing:

					// 처리할단계가 남아있으면 처리하고
					if (processes.Count > 0)
					{
						Process(delta);
					}
					// 아니면 움직임
					// 처리할 것이 있다면 처리단계에서 플레이어에게 다른 웨이포인트를 지정해 주어야 함
					// 안그러면 현재 웨이포인트에서 무한반복.
					else
					{
						Console.WriteLine("Ended~");
						ended = true;
					}

					break;

				case PlayerState.Moving:
					player.Move(delta * 150);
					break;

				case PlayerState.CheckingBomb:
					// 처리할단계가 남아있으면 처리하고
					if (processes.Count > 0)
					{
						Process(delta);
					}
					
					checkingDelta += delta;
					if (checkingDelta > 1)
					{
						player.State = PlayerState.Moving;
						var script = new Script (
										"이대원",
										"폭탄이 있습니다.",
										null,
										"find_bomb"
									);

						processes.Add (
							new DialogProcess (
								app,
								scriptView,
								new Talking (
									)
								)
							);
						break;
					}
					
					break;

				case PlayerState.CheckingMistery:
					// 처리할단계가 남아있으면 처리하고
					if (processes.Count > 0)
					{
						Process(delta);
					}

					checkingDelta += delta;
					if (checkingDelta > 1)
					{
						player.State = PlayerState.Moving;
						var script = new Script(
										"이대원",
										"폭탄이 있습니다.",
										null,
										"find_bomb"
									);

						processes.Add(
							new DialogProcess(
								app,
								scriptView,
								new Talking(
									)
								)
							);
						break;
						break;
					}
					break;
			}
		}

		// 구조 사운드가 들리는 범위 내로 들어가면 
		// 구조음이 들리게 하는 업데이트 코드들
		private void UpdateReliefSound(float delta)
		{
			if(map.Survivors == null) return;

			currPlayerPos = player.CurrentPosition;
			playerNormal = player.Normal;
			listenerPos.X = currPlayerPos.X / SoundDistanceRate;
			listenerPos.Y = currPlayerPos.Y / SoundDistanceRate;

			app.Sound.SetListenerPosition(
					listenerPos.X,
					listenerPos.Y,
					0,
					playerNormal.X,
					playerNormal.Y,
					0
				); 

			var pos = player.CurrentPosition;

			foreach (var surv in map.Survivors)
			{
				if (surv.IsPlaying)
				{
					surv.LoopDelta += delta;
					if (surv.LoopDelta > surv.LoopCycle)
					{
						surv.IsPlaying = false;
						surv.LoopDelta = 0;
					}
					continue;
				}
				// 사운드 재생할게 없음
				if (surv.Relief == null)
					continue;
				
				// 이미 구했음 ㅋ
				var way = map.GetWaypoint(surv.Id);
				if (way.Used) continue;


				float dx = pos.X - surv.X,
					dy = pos.Y - surv.Y;
				float len2 = dx * dx + dy * dy;

				if (len2 < surv.DetectRadius * surv.DetectRadius)
				{
					Console.WriteLine(
						"Play3D {0} LISTEN({1}, {2}) SOURCE({3}, {4})",
						surv.Relief,
						listenerPos.X,
						listenerPos.Y,
						surv.SX,
						surv.SY
						);
					app.Sound.Play3D(surv.Relief, surv.SX, surv.SY, 0);
					

					//app.Sound.Play2D(surv.Relief);
					/*
					var source = app.Sound.GetSoundSource(surv.Relief);
					app.Sound.Play3D(source,
									surv.SX,
									surv.SY,
									0,
									false,
									false,
									false);
					*/
					surv.IsPlaying = true;
					surv.LoopDelta = 0;
				}
			}
		}

		private void CheckAround(float delta)
		{
			Vector2 curr = player.CurrentPosition;


			foreach (var mis in map.Misteries)
			{
				if (mis.Bomb)
				{
					mis.Delta += delta;

					if (mis.Delta > mis.Time)
					{
						mis.Fired = true;
						// TODO: BOOM!!!!!!!

						app.Play3D("bomb", mis.X, mis.Y);

						if (mis.IsDamaged(curr.X, curr.Y))
						{
							Terminate();
						}

						continue;
					}
				}

				// Known 은 인식, 즉 범위에 들어갔다가 나온 경우에는
				// 다시 Known은 fasle가 됨.
				if (!mis.Known && mis.IsDetected(curr.X, curr.Y))
				{
					mis.Known = true;
					// TODO: GO CHECK!

					if(mis.Bomb)
						player.State = PlayerState.CheckingBomb;
					else
						player.State = PlayerState.CheckingMistery;
				}
				else
				{
					mis.Known = false;
				}
			}
		}

		private void Terminate()
		{
		}

		Matrix3x2 matrix;
		SharpDX.Vector2 center;

		public void Draw()
		{
			var rt = app.Graphics2D.RenderTarget;
			rt.BeginDraw();
			rt.Clear(bg);

			container.Draw();

			float scale = (float)(Math.Sin(playerAngle) / 5 + 1);
			Matrix3x2.Scaling(scale, scale, ref center, out matrix);
			matrix *= Matrix3x2.Rotation(playerAngle, center);

			app.Graphics2D.RenderTarget.Transform = matrix;
			app.Graphics2D.DrawCenter(playerBitmap, app.Width / 2, app.Height / 2);

			if (pingDelta < 1)
			{
				scale = pingDelta * 1.5f;
				Matrix3x2.Scaling(scale, scale, ref center, out matrix);

				app.Graphics2D.RenderTarget.Transform = matrix;
				app.Graphics2D.DrawCenter(ping, app.Width / 2, app.Height / 2);
			}
	
			app.Graphics2D.RenderTarget.Transform = Matrix3x2.Identity;

			rt.EndDraw();

			app.Graphics3D.Present();
		}




		public Stage getNextStage()
		{
			return next;
		}


		public bool IsEnded()
		{
			return ended;
		}


	}
}
