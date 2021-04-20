#pragma once
#include "Mono.h"
#include <cstdint>
struct RuntimeObject
{
    int handle;
    int unk;
    RuntimeObject* self;
    int* unk1;
    char gap[32];
    int* unk2;
    int pos1;
    int posX;
    int pos2;
    int posY;
};
struct RuntimeObjectInfo
{
    short handle;
    short objectType;
    int unk2;
    int unk3;
    int unk4;
    wchar_t* name;
};
struct ObjectList
{
    int objectCount;
    int handleCount;
    int* handles;
    RuntimeObjectInfo** objects;

};
struct Frame
{
    int width;
    int height;
    int objMask;
    int unk2;
    wchar_t* name;
    char gap[0x8BC];
    uintptr_t objList;
};

struct Keys {
    short up;
    short down;
    short left;
    short right;
    short button1;
    short button2;
    short button3;
    short button4;

};

struct PlayerControl {
    short controlType;
    Keys keys;
};

struct Controls {
    PlayerControl playercontrol[4];
};

struct AppHeader {
    int size;
    short flags;
    short new_flags;
    short graphics_mode;
    short other_flags;
    short windowWidth;
    short windowHeight;
    int initialScore;
    int initialLives;
    Controls controls;
    int borderColor;
    int numberOfFrames;
    int frameRate;
    char windowsMenuIndex;

};
struct Extension {
    short size;
    short handle;
    int magic;
    int versionLS;
    int versionMS;
    wchar_t* name;
    wchar_t* subtype;

};
struct ExtendedHeader {
    int flags;
    int buildType;
    int buildFlags;
    short screenRatioTolerance;
    short screenAngle;


};
struct ExtensionList {
    short ext_count;
    short preload;
    Extension extensions[3];

};
struct Key {

    char key[128];
};
struct AppMenu {
    int hdr_size;
    int menu_offset;
    int menu_size;
    int accel_offset;
    int accel_size;

};
struct DataStruct
{
    char head[4];
    short runtime_version;
    short runtime_subversion;
    int product_version;
    int product_build;
    AppHeader APPHEADER;
    wchar_t* APPNAME;
    wchar_t* EXECPATH;
    wchar_t* APPEDITORFILENAME;
    wchar_t* COPYRIGHT;
    wchar_t* ABOUTTEXT;
    wchar_t* APPTARGETFILENAME;
    char* gap98;
    HANDLE file_handle;
    unsigned int dwordA0;
    LPSTR* APPDOC;
    BITMAPINFO* AppIcon;
    HDC Icon;
    HMENU menu;
    int accel;
    wchar_t* field_B8;
    int APPMENUIMAGES;
    unsigned int APPMENU;
    unsigned int dwordC4;
    unsigned int dwordC8;
    void* FRAMEHANDLES;
    void* dwordD0;
    void* pvoidD4;
    ExtensionList EXTENSIONLIST;
    char field_F4;
    char field_F5;
    char field_F6;
    char field_F7;
    char field_F8;
    char field_F9;
    char field_FA;
    char field_FB;
    char field_FC;
    char field_FD;
    char field_FE;
    char field_FF;
    char field_100;
    char field_101;
    char field_102;
    char field_103;
    char field_104;
    char field_105;
    char field_106;
    char field_107;
    char field_108;
    char field_109;
    char field_10A;
    char field_10B;
    int unknown_offset;
    int field_110;
    int field_114;
    int field_118;
    int field_11C;
    int field_120;
    char SecNum[132];
    unsigned int dword1A8;
    char gap1AC[4];
    unsigned int dword1B0;
    char gap1B4[16];
    unsigned int dword1C4;
    char gap1C8[8];
    HWND phwnd__1D0;
    unsigned int dword1D4;
    char gap1D8[16];
    unsigned int dword1E8;
    char gap1EC[4];
    unsigned int dword1F0;
    char gap1F4[76];
    unsigned int dword240;
    unsigned int dword244;
    unsigned int dword248;
    unsigned int dword24C;
    unsigned int dword250;
    char gap254[12];
    unsigned int GLOBALVALUES;
    char gap264[8];
    Key* decode_key_26C;
    char gap270[4];
    void* GLOBALSTRINGS;
    char gap278[232];
    unsigned int dword360;
    char gap364[24];
    ExtendedHeader* APPHEADER2;
    unsigned int SPACER;
    char unicode;
    char gap385[3];
    unsigned int Shaders;
    char gap38C[4];
    char byte390;
    char gap391[7];
    char* FRAMEBANK;
    char char39C;
    int gap39D;
    char field_3A1;
    int field_3A2;
    short field_3A6;
    int TITLE2;
    char byte3AC;
    char gap3AD[7];
    unsigned int dword3B4;
    char byte3B8;
};
