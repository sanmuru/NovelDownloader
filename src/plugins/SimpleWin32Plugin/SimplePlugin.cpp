#pragma once
#include "pch.h"
#include "SimplePlugin.h"

#include <sstream>
#include <time.h>

using namespace std;

LPWSTR stringToLPCWSTR(string orig)
{
    size_t origsize = orig.length() + 1;
    const size_t newsize = 100;
    size_t convertedChars = 0;
    wchar_t* wcstring = (wchar_t*)malloc(sizeof(wchar_t) * (orig.length() - 1));
    mbstowcs_s(&convertedChars, wcstring, origsize, orig.c_str(), _TRUNCATE);

    return wcstring;
}

void RandomBook::init() {
    if (this->initiated) return;
    this->initiated = true;

    static bool randomized = false;
    if (false == randomized) {
        srand(time(0));
        randomized = true;
    }
    int randint = rand();
    stringstream ss;
    ss << "Anonymous No." << randint;
    this->Author = stringToLPCWSTR(ss.str());
    ss.str("");

    ss << "Description about <" << this->Author << ">";
    this->Description = stringToLPCWSTR(ss.str());
    ss.str("");

    ss << "The No." << randint << "Book";
    this->Title = stringToLPCWSTR(ss.str());
    ss.str("");

    LPWSTR tags[] = { (LPWSTR)L"test", (LPWSTR)L"simple" };
    this->Tags = tags;
    this->lenTags = end(tags) - begin(tags);

    this->lenVolumes = rand() % 5 + 2;
    this->Volumes = (RandomVolume*)malloc(sizeof(RandomVolume) * this->lenVolumes);
    for (int i = 0; i < this->lenVolumes, i++) {
        RandomVolume* volume = new RandomVolume();

        ss << "Volume" << i;
        volume->Title = stringToLPCWSTR(ss.str());
        ss.str("");
        volume->Book = this;
        this->Volumes[i] = *volume;
    }
}

void RandomVolume::init() {
    this->lenChapters = rand() ^ 100;
    this->Chapters = (RandomChapter*)malloc(sizeof(RandomChapter) * this->lenChapters);
    for (int i = 0; i < this->lenChapters, i++) {
        RandomChapter* chapter = new RandomChapter();

        this->lenChapters = rand() % 10 + 5;
        this->Chapters = (RandomChapter*)malloc(sizeof(RandomChapter) * this->lenChapters));
    }
}