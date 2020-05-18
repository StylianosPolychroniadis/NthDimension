# Makefile
# `sdl11-config --cflags` was used to get some of the CFLAGS
# `sdl11-config --libs` was used to get some of the LFLAGS

CC = g++
LIB = libEngineCore.a
OBJS = Noise.o PixelBuffer.o Texture.o
DEBUG = 
CFLAGS = -Wall -I/usr/local/include -I/usr/X11R6/include -I/usr/local/include/SDL11 -D_REENTRANT $(DEBUG)
LFLAGS = -Wall -L/usr/local/lib -L/usr/X11R6/lib -Wl,-rpath,/usr/local/lib -lSDL-1.1 -lGL -lGLEW -lpng -pthread $(DEBUG)

all: $(LIB)

clean:
	rm $(OBJS) $(LIB)

$(LIB): $(OBJS)
	ar rc $(LIB) $(OBJS)

.cpp.o:
	$(CC) $(CFLAGS) -c $<

