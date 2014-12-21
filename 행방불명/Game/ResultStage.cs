using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 행방불명.Framework;
using SharpDX.DirectWrite;
using SharpDX.Direct2D1;
using SharpDX;
using 행방불명.Framework.UI;

namespace 행방불명.Game
{
	public class ResultStage
		: Stage
	{
		Program app;
		Player player;
		bool ended = false;

		string rank;
		int score;
		bool success;

		Container container;
		SolidColorBrush brush;
		TextFormat format;
		Bitmap background, rankBitmap;
		string timeText;


		public ResultStage(Program app, Player player)
		{
			this.app = app;
			this.player = player;
		}

		private string InitTime()
		{
			int time = (int)(player.ElapsedTime);
			int minutes = time / 60;
			int seconds = time % 60;


			StringBuilder text = new StringBuilder();
			if (minutes < 10)
				text.Append('0');
			text.Append(minutes);
			text.Append("분 ");

			if (seconds < 10)
				text.Append('0');
			text.Append(seconds);
			text.Append("초");

			return text.ToString();
		}


		private void ComputeRankAndScore()
		{
			score = player.NumObtained * 3 - player.NumPatients;
			score -= player.NumDead * 2;
			score += (int)(Math.Max(0, 600 - Math.Max(player.ElapsedTime, 180)) / 30.0f);

            if (player.LockGasValve)
                score += 3;

            if (!success)
            {
                score -= 10;
            }
            if (score < 0) score = 0;

            if (score >= 20)
                rank = "S";
            else if (score >= 17)
                rank = "A";
            else if (score >= 14)
                rank = "B";
            else if (score >= 10)
                rank = "C";
            else
                rank = "F";
		}


		private void InitUI()
		{
			if (success)
				background = app.Media.BitmapDic["ending_success"];
			else
				background = app.Media.BitmapDic["ending_failure"];

			if (success)
				rankBitmap = app.Media.BitmapDic["rank_" + rank.ToLower()];
			else
				rankBitmap = null;

			format = app.Media.FormatDic["default32"];
			brush = new SolidColorBrush(app.Graphics2D.RenderTarget, new SharpDX.Color4(1, 1, 1, 1));
			container = new Container(app);


			timeText = InitTime();
			Console.WriteLine("TIME: " + timeText);
			var timeUI = MakeText();
			var rescuer = MakeText();
			var patients = MakeText();
			var dead = MakeText();


			timeUI.Rect = new RectangleF(457, 572, 1024, 768);
			rescuer.Rect = new RectangleF(193, 343, app.Width, app.Height);
			patients.Rect = new RectangleF(539, 343, app.Width, app.Height);
			dead.Rect = new RectangleF(873, 343, app.Width, app.Height);


			timeUI.Text = timeText;
			rescuer.Text = player.NumObtained.ToString();
			patients.Text = player.NumPatients.ToString();
			dead.Text = player.NumDead.ToString();

			timeUI.Layout.SetFontSize(64, new TextRange(0, timeUI.Text.Length));
			rescuer.Layout.SetFontSize(64, new TextRange(0, rescuer.Text.Length));
			patients.Layout.SetFontSize(64, new TextRange(0, patients.Text.Length));
			dead.Layout.SetFontSize(64, new TextRange(0, dead.Text.Length));


			container.Views.Add(timeUI);
			container.Views.Add(rescuer);
			container.Views.Add(patients);
			container.Views.Add(dead);
		}

		private TextView MakeText()
		{
			var textView = new TextView(app.Graphics2D, format, brush);
			return textView;
		}
		
		
		public void Start()
		{
			success = !player.Dead;

			// UI 초기화
			ComputeRankAndScore();
			InitUI();

			// 랭킹 추가
			app.Config.RankList.Add(new Rank()
			{
				Level = "" + rank,
				NumObtainedPeople = player.NumObtained,
				Score = score,
				Time = player.ElapsedTime
			});
			app.Config.sort();

			app.VoiceControl.Recognize();
		}


		public void Update(float delta)
		{
			container.Update(delta);

			var voice = app.VoiceControl;

			if (voice.isSuccess)
			{
				if (voice.Text.Equals("확인했습니다"))
					ended = true;
				else
					voice.Recognize();
			}
			else if (!voice.IsRecognizing)
				voice.Recognize();
		}

		public void Draw()
		{
			var rt = app.Graphics2D.RenderTarget;
			rt.BeginDraw();

			app.Graphics2D.Draw(background, 0, 0);
			container.Draw();
			
			if(rankBitmap != null)
				app.Graphics2D.DrawCenter(rankBitmap, 820, 192);

			rt.EndDraw();
			app.Graphics3D.Present();
		}

		public Stage getNextStage()
		{
			return new StartStage(app);
		}

		public bool IsEnded()
		{
			return ended;
		}
	}
}
