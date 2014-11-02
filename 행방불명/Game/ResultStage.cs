using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 행방불명.Framework;
using SharpDX.DirectWrite;
using SharpDX.Direct2D1;


namespace 행방불명.Game
{
	public class ResultStage
		: Stage
	{
		Program app;
		Player player;
		int score;
		char rank;
		TextLayout layout;
		TextFormat format;
		SolidColorBrush brush;
		bool ended = false;

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


		public void Start()
		{

			float timeScore = (900 - player.ElapsedTime) / 60.0f;
			if (timeScore < 0)
				timeScore = 0;
			score = (int)(timeScore + player.NumObtained * 2 - player.NumDead);
			brush = new SolidColorBrush(
					app.Graphics2D.RenderTarget,
					new SharpDX.Color4(1, 1, 1, 1)
				);


			if (score > 18)
				rank = 'S';
			else if (score > 14)
				rank = 'A';
			else if (score > 10)
				rank = 'B';
			else if (score > 8)
				rank = 'C';

			timeScore = ((int)(timeScore * 100)) / 6000.0f;

			StringBuilder text =new StringBuilder();
			text.Append(player.Dead ? "사망" : "성공").Append('\n')
				.Append("구조자: " + player.NumObtained).Append('\n')
				.Append("부상자: " + player.NumPatients).Append('\n')
				.Append("사망자: " + player.NumDead).Append('\n')
				.Append("소요시간: " + InitTime()).Append('\n')
				.Append("등급: " + rank).Append('\n')
				.Append("확인했습니다 - 라고 말하세요")
				;

			format = app.Media.FormatDic["default32"];
			layout = new TextLayout (
				app.Graphics2D.DWriteFactory,
				text.ToString(),
				format,
				app.Width,
				app.Height
				);
			layout.ParagraphAlignment = ParagraphAlignment.Center;
			layout.TextAlignment = TextAlignment.Center;

			var range = new TextRange(0, 2);
			layout.SetFontSize(72, range);
			layout.SetFontWeight(FontWeight.Bold, range);

			app.VoiceControl.Recognize();

			app.Config.RankList.Add(new Rank()
			{
				Level = "" + rank,
				NumObtainedPeople = player.NumObtained,
				Score = score,
				Time = player.ElapsedTime
			});
			app.Config.sort();
		}

		public void Update(float delta)
		{
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
			rt.DrawTextLayout(new SharpDX.Vector2(0, 0), layout, brush);
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
