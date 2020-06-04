.bss
    readed:
        .byte 
.data
    newline:
        .byte 0xa
    .include "defs.h"


.section .text

       
.global _start

  
_start:
    loop: 
        movq    $SYS_READ,  %rax
        movq    $STDIN,     %rdi
        movq    $readed,    %rsi
        movq    $1,         %rdx
        syscall
        
        cmpq    $0xa,      (%rsi)
        je      end_loop
        
        addb    $13, readed
        
        movq    $SYS_WRITE, %rax
        movq    $STDOUT,    %rdi
        movq    $readed,    %rsi
        movq    $1,         %rdx
        syscall
        
        jmp loop
    
    end_loop:
        movq    $SYS_WRITE, %rax
        movq    $STDOUT,    %rdi
        movq    $newline,   %rsi
        movq    $1,         %rdx
        syscall
    
    
        movq    $SYS_EXIT,  %rax
        movq    $0,         %rdi 
        syscall
    