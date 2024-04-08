#pragma once

#ifndef MYDSA_H
#define MYDSA_H

# include <stdio.h>  
# include <fstream>  
# include <string.h>  
# include <iostream>  
# include <stdlib.h>  

#define CODESIZE			8
#define GARANTYSIZE			4
#define OPTSIZE				4
#define HEADERFIELDS		5			// DATA(0) PLOTTER(1) DEALER(2) OPTIONS(3) CHECKSUM(4)

using namespace std;

class Des
{
public:
    int keyi[16][48],
        total[64],
        left[32],
        right[32],
        ck[28],
        dk[28],
        expansion[48],
        z[48],
        xor1[48],
        sub[32],
        p[32],
        xor2[32],
        temp[64],
        pc1[56],
        ip[64],
        inv[8][8];

    unsigned char final[1000];
    void IP();
    void PermChoice1();
    void PermChoice2();
    void Expansion();
    void inverse();
    void xor_two();
    void xor_oneE(int);
    void xor_oneD(int);
    void substitution();
    void permutation();
    void keygen();
    unsigned char* Encrypt(unsigned char*, int, char* id = NULL);
    unsigned char* Decrypt(unsigned char*, int);
};
#endif