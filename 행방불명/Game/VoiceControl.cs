using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Speech.Recognition;
using System.Windows.Forms;


namespace 행방불명.Game
{
	public class VoiceControl
	{
		SpeechRecognitionEngine engine;
		bool recoging = false, success = false;
		string resultText = null;
		RecognitionResult result;

		int numLoading = 0;

		public bool isSuccess { get { return success; } }
		public bool IsLoading { get { return numLoading == 0 ;} }
		public bool IsRecognizing { get { return recoging; } }
        public string Text { get { return resultText; } set { resultText = value; } }
		public RecognitionResult Result { get { return result; } }
		public SpeechRecognitionEngine Engine { get { return engine; } }


		public VoiceControl()
		{
			try
			{

				engine = new SpeechRecognitionEngine(
					new System.Globalization.CultureInfo("ko-kr")
					);
				engine.SetInputToDefaultAudioDevice();

				Console.WriteLine(
					"Speech Engine " +
					engine.RecognizerInfo.Name +
					" " + engine.RecognizerInfo.Culture +
					" " + engine.RecognizerInfo.Id
					);

				engine.LoadGrammarCompleted += (object sender, LoadGrammarCompletedEventArgs args) =>
				{
					numLoading--;
					Console.WriteLine("입력 문법 로딩 완료, {0}개 남음.", numLoading);
				};
				engine.RecognizeCompleted += (object sender, RecognizeCompletedEventArgs args) =>
				{
					if (args.Result != null)
					{
						resultText = args.Result.Text;
						success = true;

						Console.WriteLine("Voice: " + resultText);
					}
					else
					{
						Console.WriteLine("Voice(failed)");
						success = false;
					}
					result = args.Result;
					recoging = false;
				};
			}
			catch (Exception e)
			{
				MessageBox.Show("마이크가 연결되어 있지 않습니다.", "에러");
				if (System.Windows.Forms.Application.MessageLoop)
				{
					// WinForms app
					System.Windows.Forms.Application.Exit();
				}
				else
				{
					// Console app
					System.Environment.Exit(1);
				}
			}
		}

		~VoiceControl()
		{
			engine.Dispose();
		}

		public void Reset()
		{
			resultText = null;
			recoging = success = false;

			engine.UnloadAllGrammars();
		}


		public void Recognize()
		{
			if (recoging) return;
			Console.WriteLine("Recognition started!");

			recoging = true;
			success = false;
			resultText = "";
			engine.RecognizeAsync();
		}


		public void Cancle()
		{
			engine.RecognizeAsyncCancel();
		}

		public void Load(string [][]choicesText)
		{
			var builder = new GrammarBuilder();
			foreach (string[] item in choicesText)
			{
				Choices choices = new Choices();
				foreach (string choice in item)
				{
					choices.Add(choice);
				}
				builder.Append(choices);
			}

			var grammar = new Grammar(builder);
			numLoading ++;
			engine.LoadGrammarAsync(grammar);
		}

		public void Load(Choices choice)
		{
			var builder = new GrammarBuilder();
			builder.Append(choice);

			var grammar = new Grammar(builder);
			numLoading++;
			engine.LoadGrammarAsync(grammar);
		}
	}
}
