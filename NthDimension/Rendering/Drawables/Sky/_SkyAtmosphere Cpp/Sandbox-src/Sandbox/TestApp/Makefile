# Makefile
# `sdl11-config --cflags` was used to get some of the CFLAGS
# `sdl11-config --libs` was used to get some of the LFLAGS

CC = g++
EXE = ../Bin/TestApp
OBJS = TestApp.o
DEBUG = 
CFLAGS = -Wall -I/usr/local/include -I/usr/X11R6/include -I/usr/local/include/SDL11 -I../EngineCore -D_REENTRANT $(DEBUG)
LFLAGS = -Wall -L/usr/local/lib -L/usr/X11R6/lib -L../EngineCore -Wl,-rpath,/usr/local/lib -lSDL-1.1 -lGL -lGLEW -lpng -lEngineCore -pthread $(DEBUG)

all: $(EXE)

clean:
	rm $(OBJS)

$(EXE): $(OBJS)
	$(CC) $(LFLAGS) $(OBJS) -o $(EXE)

.cpp.o:
	$(CC) $(CFLAGS) -c $<


