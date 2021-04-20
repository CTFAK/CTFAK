#pragma once
#include "Structs.h"

class Loader
{
public:
	static MonoDomain* Domain;
	static MonoMethod* OnUpdateMethod;
	static MonoMethod* OnFrameSwitchMethod;
	static MonoMethod* OnInjectedMethod;
	static DataStruct* gameDataPtr;
	static uintptr_t GameBase;
	static HMODULE CTFAK;
	static void Entry();
	static void DoHooks(uintptr_t base, int gameType);
	static void InitMono();
	static void DrawUI();
};
struct CounterValue
{
	int32_t a;
	int32_t b;
	int32_t value;
	int32_t unk;
};

