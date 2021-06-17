.PHONY: all build test auto

all: build test auto

build:
	mcs Meta.cs main.cs VJP.cs option/*cs -out:meta.exe

test:
	cat test.json | mono meta.exe > AutoGen.cs && cat AutoGen.cs

auto:
	mcs main_auto.cs AutoGen.cs VJP.cs option/*cs -out:auto.exe
