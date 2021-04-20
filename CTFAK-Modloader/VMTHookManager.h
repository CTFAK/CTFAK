class VMTHookManager
{
public:
    VMTHookManager(void** vTable); //constructor
    ~VMTHookManager(); //destructor
    void* GetFunctionAddyByIndex(unsigned short index);// getting the address of a function in the vtable by index
    void* Hook(unsigned short index, void* ourFunction); // hooking the virtual function by index
    bool Unhook(unsigned short index); // unhooking the virtual function by index
    void UnhookAll(); //unhook all the functions

private:
    //member variables
    void** m_vTable; // the vtable of some object
    unsigned short m_numberFuncs; // number of virtual functions
    void** m_originalFuncs = nullptr; // we'll save the original addresses here

    unsigned short GetNumberOfFunctions(); //get the number of virtual functions
    bool toHook(unsigned short index, bool hook = false, void* ourFunction = nullptr, uintptr_t* bufferOriginalFunc = nullptr); //function used to hook/unhook
};