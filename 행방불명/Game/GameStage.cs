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
using SharpDX.DirectWrite;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json;
using Microsoft.Speech.Recognition;
using IrrKlang;
using System.Diagnostics;

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
		string mapPath;
		GameStageOptions options;

		public GameStage(
			Program app,
			string map
			)
		{
			this.app = app;
			this.bg = new Color4(0, 0, 0, 1);
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
			GameStageOptions options
			)
		{
			this.app = app;
			this.bg = new Color4(0, 0, 0, 1);
			this.mapPath = map;
			this.options = options;
		}

		public ScriptView ScriptView { get { return scriptView; } }
		public Player Player { get { return player;  } }
		public MapData Map { get { return map; } }
		public Program App { get { return app; } }
		public List<IProcess> Processes { get { return processes; } }
		public List<GameObject> GameObjects { get { return gameObjects; } }
		public GameView GameView { get { return gameView; } }


		Container container;
		ScriptView scriptView;
		GameView gameView;
		TimeView timeView;
		CountView countView;
		SettingButtons exitBtn;
		MapView mapView;

		Bitmap playerBitmap, ping, danger;
		float playerAngle = 0, pingDelta = 0;
		readonly float SoundDistanceRate = 100.0f;

		public void Start()
		{
			Console.WriteLine(mapPath + " Game Stage started.");


			playerBitmap = app.Media.BitmapDic["character"];
			ping = app.Media.BitmapDic["ping"];
			danger = app.Media.BitmapDic["danger_icon"];
			gameObjects = new List<GameObject>();

			center = new Vector2(app.Width / 2, app.Height / 2);
			voice = app.VoiceControl;
			processBuilder = new ProcessBuilder(app, this);

			LoadMap(mapPath);
			InitUI();
		}

		private void InitUI()
		{
			//player.HasHammer = true;
			//player.HasKey = true; 
            //player.NumMedicalKits = 5;
            player.ElapsedTime = 0;
			redBrush = new SolidColorBrush(
							app.Graphics2D.RenderTarget,
							new Color4(1, 0, 0, 1)
							);

			fadeoutBrush = new SolidColorBrush(
							app.Graphics2D.RenderTarget,
							new Color4(0, 0, 0, 0)
							);

			bombCountFormat = app.Media.FormatDic["default32"];
			bombCountDrawRect = new RectangleF(0, 0, app.Width, app.Height);

			container = new Container(app);
			gameView = new GameView(app);
			scriptView = new ScriptView(app);
			exitBtn = new SettingButtons(app);
			mapView = new MapView(this);


			//container.Views.Add(gameView);
			container.Views.Add(exitBtn);
			container.Views.Add(scriptView);

			if (options.HasCountUI)
			{
				countView = new CountView(app, player);
				container.Views.Add(countView);
			}
			if (options.HasMapUI)
			{
				container.Views.Add(mapView);
			}
			if (options.HasTimeUI)
			{
				timeView = new TimeView(app);
				container.Views.Add(timeView);
			}


            if(map.IsTutorial())
            {
                var escToSkipView = new ImageView(app.Graphics2D, app.Media.BitmapDic["esc_to_skip"]);
                escToSkipView.X = app.Width - escToSkipView.Width - 30;
                escToSkipView.Y = 30;
                escToSkipView.Draw += escToSkipView.DrawBitmap;
                container.Views.Add(escToSkipView);
            }
			
			
		}



		MapData map;
		List<GameObject> gameObjects;
		IProcess checkingProcess = null,
			notificatingProcess = null;

		private void SetupSurvivor(Survivor surv)
		{
			Waypoint way = map.GetWaypoint(surv.Id);
			if (way == null)
			{
				Console.WriteLine("알 수 없는 생존자의 연결 ID입니다. " + surv.Id);
			}
            
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
            surv.WX = way.X;
            surv.WY = way.Y;
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

		bool ended = false, failed = false;
		VoiceControl voice;
		Player player;
		Waypoint currWaypoint;
		List<IProcess> processes = new List<IProcess>();
		ProcessBuilder processBuilder;
		Vector2 currPlayerPos, playerNormal;
		Vector2 listenerPos = new Vector2();

		Mistery currentChecking;

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

		bool fadeout = false;
		float fadeoutRate = 0;
		SolidColorBrush fadeoutBrush;

		public void Update(float delta)
		{
			if (fadeout)
			{
				fadeoutRate += delta / 5;
				if (fadeoutRate > 1)
					ended = true;
				return;
			}

			if (map.escToSkip && app.KeyESC)
			{
				ended = true;
				return;
			}

			playerAngle += delta * 3.141592f / 4;
			player.ElapsedTime += delta;
			pingDelta += delta;
			if (pingDelta > 2)
				pingDelta = 0;
			if (playerAngle > 6.283)
				playerAngle = 0;



			UpdateReliefSound(delta);
			CheckAround(delta);

			gameView.OnUpdate(delta);
			container.Update(delta);
			gameView.Center = player.CurrentPosition;

			player.Update(delta);
			// 플레이어 상태에 따른 처리
			switch(player.State)
			{
				case PlayerState.Arrived:
                    scriptView.Unknown = null;
					processes.Clear();
					processBuilder.AddProcess(processes, player.MoveTo);
				    player.RunningAway = false;
					player.State = PlayerState.Processing;

					string sfx = player.MoveTo.Sfx;
					if (sfx != null)
						app.Play2D(sfx);
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
						Terminate(false);
					}

					break;

				case PlayerState.Moving:
					player.Move(delta * 225);
					CheckPatientsStatus(delta);
					break;

				case PlayerState.Checking:
					// 처리할단계가 남아있으면 처리하고
					if (processes.Count > 0)
					{
						Process(delta);
					}
					checkingDelta += delta;
					if (checkingDelta > 1 && !currentChecking.Checked)
					{
						checkingDelta = 0;

						checkingProcess.Update(delta);

						if (checkingProcess.IsEnded())
						{
							Console.WriteLine("CHECKED!");
							currentChecking.Checked = true;
						}
					}
					break;
				case PlayerState.Notificating:

					notificatingProcess.Update(delta);
					if (notificatingProcess.IsEnded())
					{
						notificatingProcess.End();
						player.State = PlayerState.Moving;
					}
					break;
			}
		}

        float patientWarningDelta = 30;

		private void CheckPatientsStatus(float delta)
		{
            patientWarningDelta += delta;
			int numDead = player.RemoveDeadPatients();

			string dieAlias = "survivor_die";
            if (numDead > 0)
            {
                Console.WriteLine("부상당한 생존자 {0}명 사망.", numDead);
                //app.Play2D(dieAlias);

                Script script = new Script(
                    "이대원",
                    "생존자께서 돌아가셨습니다.",
                    "",
                    dieAlias
                    );

                notificatingProcess = new DialogProcess(
                    app,
                    scriptView,
                    new Talking(
                        new Script[] { script }
                        )
                    );

                player.State = PlayerState.Notificating;
                notificatingProcess.Start();
                return;

            }
            if (patientWarningDelta <= 30) return;

            foreach(float life in player.PatientsLife)
            {
                if (life > 30) continue;

                patientWarningDelta = 0;
                Console.WriteLine("경고: 부상자가 위험함");
                //app.Play2D(dieAlias);

                Script script = new Script(
                    "이대원",
                    "환자분이 위급합니다.",
                    "",
                    "patient_dangerous"
                    );

                notificatingProcess = new DialogProcess(
                    app,
                    scriptView,
                    new Talking(
                        new Script[] { script }
                        )
                    );

                player.State = PlayerState.Notificating;
                notificatingProcess.Start();
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
				
				// 이미 구했음 ㅋ
				var way = map.GetWaypoint(surv.Id);
				if (way.Used) continue;


				float dx = pos.X - surv.X,
					dy = pos.Y - surv.Y;
				float len2 = dx * dx + dy * dy;

                // 구조 요청 재생
				if (len2 < surv.DetectRadius * surv.DetectRadius)
                {
                    if (!surv.IsFound) 
                        app.Sound.Play2D("notification");

                    surv.IsFound = true;

                    // 사운드 재생할게 없음
                    if (surv.Relief == null)
                        continue;
					Console.WriteLine(
						"Play3D {0} LISTEN({1}, {2}) SOURCE({3}, {4})",
						surv.Relief,
						listenerPos.X,
						listenerPos.Y,
						surv.SX,
						surv.SY
						);
                    way.Thanks = app.Sound.Play3D(surv.Relief, surv.SX, surv.SY, 0);
                    
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

	
		
		Mistery currCheckingAroundMistery;
		private void CheckAround(float delta)
		{
			CheckGameObjects();
			Vector2 curr = player.CurrentPosition;


			foreach (var mis in map.Misteries)
			{
				if (mis.Fired) continue;
				currCheckingAroundMistery = mis;

				if (mis.Bomb)
				{
					mis.Delta += delta;

					if (mis.Delta > mis.Time && !mis.Fired)
					{
						mis.Fired = true;
						// TODO: BOOM!!!!!!!

						float sx = mis.X / SoundDistanceRate,
							sy = mis.Y / SoundDistanceRate;

						var dx = listenerPos.X - sx;
						var dy = listenerPos.Y - sy;
						var len2 = dx * dx + dy * dy;

						if(len2 < 18)
						{
							app.Play2D("bomb");
						}

						Console.WriteLine(
							"펑! {0} LISTEN({1}, {2}) SOURCE({3}, {4})",
							mis.Id,
							listenerPos.X,
							listenerPos.Y,
							sx,
							sy
							);

						if (mis.IsDamaged(curr.X, curr.Y))
						{
							player.Dead = true;
							Terminate(true);
						}

						continue;
					}
				}

				// Known 은 인식, 즉 범위에 들어갔다가 나온 경우에는
				// 다시 Known은 fasle가 됨.
				if (mis.IsDetected(curr.X, curr.Y) && player.State == PlayerState.Moving
					&& !mis.Known)
				{
					mis.Known = true;
					currentChecking = mis;

					Console.WriteLine("GO CHECK!");

					player.State = PlayerState.Processing;



					var dynamic =
						new DynamicSelectionProcess(
							this,
							new DynamicSelectionHandler(this, mis).Handle,
							new Script(
									"이대원",
									"수상한 물건이 있습니다.",
									null,
									"find_mistery"
								),
							new string[] { "살펴봐", "무시해" }
							);

					processes.Add(dynamic);
					dynamic.Start();
				}
			}
			currCheckingAroundMistery = null;
		}


		class DynamicSelectionHandler
		{
			Mistery mis;
			GameStage stage;

			public DynamicSelectionHandler(GameStage stage, Mistery mis)
			{
				this.mis = mis;
				this.stage = stage;
			}

			public IProcess Handle(string value)
			{

				Program app = stage.App;
				ScriptView scriptView = stage.ScriptView;
				List<IProcess> processes = stage.Processes;
				Player player = stage.Player;

				if (value.Equals("살펴봐"))
				{
					Script script = null;


					if (mis.Bomb)
					{
						if (mis.Time - mis.Delta < 15)
						{
							player.RunningAway = true;
							app.Play2D("runaway");
						}
						else
							script = new Script(
								"이대원",
								"폭탄이 있습니다!",
								null,
								"find_bomb"
								);
					}
					else
						script = new Script(
							"이대원",
							"별 거 아닙니다.",
							null,
                            "nothing"
							);

					var p = new DialogProcess(
						app,
						scriptView,
						new Talking(
							script
							)
						);
                    if (!player.RunningAway)
                    {
                        processes.Add(new SoundPlayProcess(app, "search"));
                        processes.Add(new DelayProcess(5));
                        if (mis.Bomb)
                            processes.Add(new CheckMiteryProcess(mis));
                        processes.Add(p);
                    }
                    else if (mis.Bomb)
                        processes.Add(new CheckMiteryProcess(mis));
				}
				else if (value.Equals("무시해"))
				{
					mis.Ignored = true;
				}

				return new MoveProcess(stage);
			}
		}
		private void CheckGameObjects()
		{

			//foreach (var obj in gameObjects)
			//{
			//}


		}

		private void Terminate(bool failed)
		{
            if(failed)
            {
                Console.WriteLine("사망"); 
                app.Sound.StopAllSounds();
                app.Play2D("game_over");
            }
			fadeout = true;
            this.failed = failed;
		}

		Matrix3x2 matrix;
		SharpDX.Vector2 center;

		public void Draw()
		{
			var rt = app.Graphics2D.RenderTarget;
			rt.BeginDraw();
			rt.Clear(bg);

			gameView.OnDraw();

            if(!failed || (!fadeout || fadeoutRate < 0.5))
            {
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

                    float opacity = pingDelta / 2.0f;
                    app.Graphics2D.DrawCenter(ping, app.Width / 2, app.Height / 2, 1 - pingDelta * pingDelta);
                }

                app.Graphics2D.RenderTarget.Transform = Matrix3x2.Identity;
            }
			
			DrawGameObjects();
			container.Draw();

			
            if (fadeout && fadeoutRate > 0.5f)
			{
				fadeoutBrush.Color = new Color4(0, 0, 0, 1);
				rt.FillRectangle(app.RectF, fadeoutBrush);
			}
            


			rt.EndDraw();

			app.Graphics3D.Present();
		}


		TextFormat bombCountFormat;
		SolidColorBrush redBrush;
		RectangleF bombCountDrawRect;

		private void DrawGameObjects()
		{
			float rx = 0, ry = 0;
			var curr = player.CurrentPosition;
			var vp = gameView.Viewport;

			foreach (var obj in gameObjects)
			{
				rx = obj.x - vp.X;
				ry = obj.y - vp.Y;

				if (rx > 0 && rx < app.Width
					&& ry > 0 && ry < app.Height)
				{
					app.Graphics2D.DrawCenter(obj.bitmap, rx, ry);
				}
			}

            if(map.Survivors != null)
            {
                var survBitmap = app.Media.BitmapDic["survivor_icon"];
                foreach (var surv in map.Survivors)
                {
                    if (!surv.IsFound) continue;

                    rx = surv.WX - vp.X;
                    ry = surv.WY - vp.Y;

                    if (rx > 0 && rx < app.Width
                        && ry > 0 && ry < app.Height)
                    {
                        app.Graphics2D.DrawCenter(survBitmap, rx, ry);
                    }
                }
            }

            foreach (var bomb in map.Misteries)
			{
				bool drawable = //bomb.Known ||
								bomb.Checked || bomb.Ignored;
				if (bomb.Fired) continue;
				if (!drawable) continue;

				rx = bomb.X - vp.X;
				ry = bomb.Y - vp.Y;

				
				if (rx > 0 && rx < app.Width
					&& ry > 0 && ry < app.Height)
				{
					if (bomb.Ignored)
					{
						app.Graphics2D.Draw(
							danger,
							rx,
							ry
							);
					}
					else
					{
						bombCountDrawRect.X = rx;
						bombCountDrawRect.Y = ry;

						app.Graphics2D.RenderTarget.DrawText(
							((int)(bomb.Time + 1 - bomb.Delta)).ToString(),
							bombCountFormat,
							bombCountDrawRect,
							redBrush
							);
					}
				}
			}
		}

		public Stage getNextStage()
		{
			if (map.IsTutorial())
				return new GameStage(app, "res/B1.json");
			else
				return new ResultStage(app, player);
		}


		public bool IsEnded()
		{
			return ended;
		}

		[Conditional("DEBUG")]
		private void PrintLog()
		{
		}
	}
}
