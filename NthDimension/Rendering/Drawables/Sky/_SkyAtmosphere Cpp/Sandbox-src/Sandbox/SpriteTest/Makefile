# Makefile
# `sdl11-config --cflags` was used to get some of the CFLAGS
# `sdl11-config --libs` was used to get some of the LFLAGS

CC = g++
EXE = ../Bin/SpriteTest
OBJS = SpriteTest.o ../EngineCore/Noise.o ../EngineCore/PixelBuffer.o ../EngineCore/Texture.o
DEBUG = 
CFLAGS = -Wall -I/usr/local/include -I/usr/X11R6/include -I/usr/local/include/SDL11 -I../EngineCore -D_REENTRANT $(DEBUG)
LFLAGS = -Wall -L/usr/local/lib -L/usr/X11R6/lib -Wl,-rpath,/usr/local/lib -lSDL-1.1 -lGL -lGLEW -lpng -pthread $(DEBUG)

all: $(EXE)

clean:
	rm *.o

$(EXE): $(OBJS)
	$(CC) $(LFLAGS) $(OBJS) -o $(EXE)

.cpp.o:
	$(CC) $(CFLAGS) -c $<


