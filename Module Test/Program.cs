using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Speech.Recognition;


namespace Module_Test
{
	class Program
	{
		static void Main(string[] args)
		{
			var engine = new SpeechRecognitionEngine(
				new System.Globalization.CultureInfo("ko-kr")
				);
			engine.SetInputToDefaultAudioDevice();

			Choices choices = new Choices();
			choices.Add("북쪽");
			choices.Add("서쪽");
			choices.Add("동쪽");
			choices.Add("남쪽");

			var builder = new GrammarBuilder(choices);
			var grammar = new Grammar(builder);
			engine.LoadGrammar(grammar);

			Console.WriteLine("start~~~~");
			while (true)
			{
				Console.WriteLine("말해보슈");
				var result = engine.Recognize();
				if (result != null && result.Text != null)
					Console.WriteLine(result.Text);
				else
					Console.WriteLine("????");
			}
		}
	}
}
