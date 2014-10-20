
#include <irrKlang.h>
#include <stdio.h>


int main(void)
{
	irrklang::ISoundEngine *engine = irrklang::createIrrKlangDevice();
	if(engine)
	{
		auto sound = engine->play2D("What Makes You Beautiful.mp3");
		
		printf("Input any text to quit.\n");
		getchar();

		engine->drop();
	}
	else
		printf("Cannot get irrklang sound device.\n");

	
	return 0;
}