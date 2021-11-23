
#include "pch.h"
#include "ndplugin.h"
#include "ndplugin.impl.h"

LPWSTR Book_GetTitle(HBOOK hBook) {
	if (hBook == nullptr) throw "argument is nullptr: hBook";

	Book* book = (Book*)hBook;

	book->init();
	return book->Title;
}

LPWSTR Book_GetAuthor(HBOOK hBook) {
	if (hBook == nullptr) throw "argument is nullptr: hBook";

	Book* book = (Book*)hBook;

	book->init();
	return book->Author;
}

int Book_GetTags(HBOOK hBook, TagIterator iterator);

LPWSTR Book_GetDescription(HBOOK hBook) {
	if (hBook == nullptr) throw "argument is nullptr: hBook";

	Book* book = (Book*)hBook;

	book->init();
	return book->Description;
}

int Book_GetVolumes(HBOOK hBook, VolumeIterator iterator);