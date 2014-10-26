using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Speech.Recognition;

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
		public string Text { get { return resultText; } }
		public RecognitionResult Result { get { return result; } }
		public SpeechRecognitionEngine Engine { get { return engine; } }


		public VoiceControl()
		{
			engine = new SpeechRecognitionEngine();
			engine.SetInputToDefaultAudioDevice();

			engine.LoadGrammarCompleted += (object sender, LoadGrammarCompletedEventArgs args)=>
			{
				numLoading --;
			};
			engine.RecognizeCompleted += (object sender, RecognizeCompletedEventArgs args) =>
			{
				if(args.Result != null)
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
