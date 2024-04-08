//#include "stdafx.h"
#include <stdio.h>
#include <dos.h>
#include <stdio.h>
#include <string.h>
#include <stdlib.h>



#define ARRAY_SIZE(a) (sizeof(a)/sizeof(a[0]))

char S[256];

void swap(char* s, unsigned int i, unsigned int j) {
	unsigned char temp = s[i];
	s[i] = s[j];
	s[j] = temp;
}

/* KSA */
void rc4_init(char* key, int key_length, int* i, int* j) {
	for (*i = 0; *i < 256; (*i)++)
		S[*i] = *i;

	for (*i = *j = 0; *i < 256; *i = *i + 1) {
		*j = (*j + key[*i % key_length] + S[*i]) & 255;
		swap(S, *i, *j);
	}

	*i = *j = 0;
}

/* PRGA */
char rc4_output(int* i, int* j) {
	*i = (*i + 1) & 255;
	*j = (*j + S[*i]) & 255;

	swap(S, *i, *j);

	return S[(S[*i] + S[*j]) & 255];
}

void personalitza_clau(char* key) {

}


int crypt(char* input, char* output, int len, char* key) {
	int i, j;

	rc4_init(key, strlen(key), &i, &j);
	int y;
	for (y = 0; y < len; y++)
		output[y] = input[y] ^ rc4_output(&i, &j);
	return (0);
}



int main() {

	char k[8];
	char t[15] = "Hola Bon dia";
	char o[16], u[16];

	sprintf(k, "%s", "CLAU");
	personalitza_clau(k);

	for (int i = 0; i < 12; i++) printf("%c ", t[i]); printf("\n");
	crypt(t, o, 12, k);
	for (int i = 0; i < 12; i++) printf("%c ", o[i]); printf("\n");
	crypt(o, u, 12, k);
	for (int i = 0; i < 12; i++) printf("%c ", u[i]); printf("\n");





	return 0;
}