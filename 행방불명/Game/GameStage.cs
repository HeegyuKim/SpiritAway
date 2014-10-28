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

namespace 행방불명.Game
{

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

		Container container;
		ScriptView scriptView;
		GameView gameView;
		TimeView timeView;
		CountView countView;
		SettingButtons exitBtn;
		Bitmap playerBitmap;
		float playerAngle = 0;


		public void Start()
		{
			Console.WriteLine(mapPath + " Game Stage started.");

			playerBitmap = app.Media.BitmapDic["character"];
			center = new Vector2(app.Width / 2, app.Height / 2);
			voice = app.VoiceControl;
			processBuilder = new ProcessBuilder(app, this);

			LoadMap(mapPath);
			InitUI(options);
		}

		private void InitUI(GameStageOptions options)
		{
			player.HasHammer = true;
			player.HasKey = true;
			player.NumMedicalKits = 5;

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

		public void Update(float delta)
		{
			playerAngle += delta * 3.141592f / 4;
			if (playerAngle > 6.283)
				playerAngle = 0;

			container.Update(delta);
			gameView.Center = player.CurrentPosition;

			// 플레이어 상태에 따른 처리
			switch(player.State)
			{
				case PlayerState.Arrived:
					processBuilder.AddProcess(processes, player.MoveTo);
					currWaypoint.Used = true;
					player.State = PlayerState.Processing;
					break;

				case PlayerState.Processing:

					// 처리할단계가 남아있으면 처리하고
					if (processes.Count > 0)
					{
						var p = processes.First();
						p.Update(delta);

						// 다 처리됬으면 없애기
						if (p.IsEnded())
						{
							p.End();
							processes.RemoveAt(0);
							if (processes.Count > 0)
								processes.First().Start();
						}
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
			}
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
