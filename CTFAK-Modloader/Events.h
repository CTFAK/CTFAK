#pragma once
struct Parameter
{
	short size;
	short code;
};
struct Condition
{
	short size;
	short objectType;
	short Num;
	short objectInfo;
	short objectInfoList;
	short flags;
	short otherFlags;
	char paramCount;
	char unk;
	//Parameter params[this.paramCount];

};
struct Action
{
	short size;
	short objectType;
	short Num;
	short objectInfo;
	short objectInfoList;
	short flags;
	char paramCount;
	char unk;
};
struct EventPair
{
	Condition cond;
	Action act;
};
struct Event
{
	short size;
	short unk2;
	short zero;
	short zero2;
	short zero3;
	short zero4;
	short zero5;
	short zero6;
	EventPair evts[2];

};