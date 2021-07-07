#pragma once
#include "Mono.h"
#include "MMF\Ccxhdr.h"

class Loader
{
public:
	static MonoDomain* Domain;
	static MonoMethod* OnUpdateMethod;
	static MonoMethod* OnFrameSwitchMethod;
	static MonoMethod* OnInjectedMethod;
	static CRunApp* currentApp;
	static uintptr_t GameBase;
	static HMODULE CTFAK;
	static void Entry();
	static void DoHooks(uintptr_t base, int gameType);
	static void InitMono();
	static void DrawUI();
};


