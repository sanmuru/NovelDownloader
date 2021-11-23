#include <sstream>
#include <string>
#include <Windows.h>
#include <cstdarg>
#include <stdio.h>

using std::stringstream;

LPCWSTR StringFormat(int cnt, ...) {
    stringstream ss;
    if (cnt <= 0) return L"";

    va_list args;
    va_start(args, cnt);
    for (int i = 0; i < cnt; i++) {
        
    }
}

LPCWSTR stringToLPCWSTR(std::string orig)
{
    size_t origsize = orig.length() + 1;
    const size_t newsize = 100;
    size_t convertedChars = 0;
    wchar_t* wcstring = (wchar_t*)malloc(sizeof(wchar_t) * (orig.length() - 1));
    mbstowcs_s(&convertedChars, wcstring, origsize, orig.c_str(), _TRUNCATE);

    return wcstring;
}