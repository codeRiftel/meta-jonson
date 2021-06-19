.PHONY: all build test auto

all: build test_to test_from auto_to auto_from

to: build test_to auto_to

from: build test_from auto_from

build:
	mcs Meta.cs main.cs VJP.cs option/*cs -out:meta.exe

test_to:
	mono meta.exe to < description.json > AutoGenTo.cs && cat AutoGenTo.cs

test_from: test_to
	mono meta.exe from < description.json > AutoGenFrom.cs && cat AutoGenFrom.cs

auto_to:
	mcs main_to.cs Person.cs AutoGenTo.cs VJP.cs option/*cs -out:auto_to.exe

auto_from:
	mcs main_from.cs Person.cs AutoGenTo.cs AutoGenFrom.cs VJP.cs option/*cs -out:auto_from.exe
