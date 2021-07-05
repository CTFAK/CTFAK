
#ifndef _MvtExt_h
#define _MvtExt_h

#ifdef MVTEXT_EXPORTS
#define MVTEXT_API __declspec(dllexport)
#else
#define MVTEXT_API __declspec(dllimport)
#endif

#include "Props.h"
#include "Mvt.h"

extern HINSTANCE hInst;

#endif // _MvtExt_h
