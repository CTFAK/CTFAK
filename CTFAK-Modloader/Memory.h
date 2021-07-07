#pragma once
#include <string.h>
#include <stdlib.h>
#include <vector>
namespace Memory
{
	int str_to_bytes(const char* pattern, int* bytes) {

		auto start = const_cast<char*>(pattern);
		auto end = const_cast<char*>(pattern) + strlen(pattern);
		int c = 0;
		for (auto current = start; current < end; ++current) {
			if (*current == '?') {
				++current;
				if (*current == '?')
					++current;
				bytes[c] = -1;
			}
			else {
				bytes[c] = strtoul(current, &current, 16);
			}
			c++;
		}
		return c - 1;
	};

	void* aob(uintptr_t addrbase, unsigned long sizeOfImage, const char* pattern) {

		int bytes[300];
		auto s = str_to_bytes(pattern, bytes);
		auto scanBytes = reinterpret_cast<unsigned char*>(addrbase);

		for (auto i = 0ul; i < sizeOfImage - s; ++i) {
			bool found = true;
			for (auto j = 0ul; j < s; ++j) {
				if (scanBytes[i + j] != bytes[j] && bytes[j] != -1) {
					found = false;
					break;
				}
			}
			if (found) {
				return &scanBytes[i];
			}
		}

		return nullptr;
	}
	uintptr_t FindDMAAddy(uintptr_t ptr, std::vector<unsigned int> offsets)
	{
		uintptr_t addr = ptr;
		for (unsigned int i = 0; i < offsets.size(); ++i)
		{
			addr = *(uintptr_t*)addr;
			addr += offsets[i];
		}
		return addr;
	}
}
