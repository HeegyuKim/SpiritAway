// TestSpeech.cpp : �⺻ ������Ʈ �����Դϴ�.

#include "stdafx.h"
#include <stdlib.h>

using namespace System;
using namespace Microsoft::Speech;
using namespace Microsoft::Speech::Recognition;

int main(array<System::String ^> ^args)
{
	Console::WriteLine("Installed speech languages.");
	for each (RecognizerInfo ^info in SpeechRecognitionEngine::InstalledRecognizers())
	{
		Console::WriteLine(info->Culture);
	}

	SpeechRecognitionEngine ^engine = nullptr;
	try 
	{
		engine = gcnew SpeechRecognitionEngine(
				gcnew System::Globalization::CultureInfo("ko-KR"));
		
		Grammar ^grammar = gcnew Grammar("Config.xml");

		engine->LoadGrammar(grammar);
		engine->SetInputToDefaultAudioDevice();
		while(true)
		{
			Console::WriteLine("���غ�����.");

			auto result = engine->Recognize();

			Console::WriteLine(result->Text);
		}
	}
	catch(Exception^ e)
	{
		Console::WriteLine(e->Message);
		Console::WriteLine(e->StackTrace);
	}

	system("pause");
	return 0;
}
