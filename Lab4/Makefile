HEADERS			=		defs.h
OUT				=		task
OBJECTS			=		$(patsubst %.s, %.o, $(wildcard *.s))
ASFLAGS			= --gdwarf-2
LDFLAGS			= -static

default: $(OUT)
.PHONY: clean all $(OUT) default

$(OUT): $(OBJECTS) $(HEADERS)
	@$(LD) $(LDFLAGS) -o $(OUT) $(OBJECTS)

all: $(OUT)

clean: 
	@rm -f *.o $(OUT)


