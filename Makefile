# Copyright (c) 2014, Facebook, Inc.
# All rights reserved.
#
# This source code is licensed under the BSD-style license found in the
# LICENSE file in the root directory of this source tree. An additional grant
# of patent rights can be found in the PATENTS file in the same directory.

FILES=src/__tests__/Layout-test.c src/Layout.c src/Layout-test-utils.c

JAVA_CP:=./lib/junit4.jar:./lib/jsr305.jar:./lib/infer-annotations-1.4.jar
JAVA_SP:=./src/java/src:./src/java/tests 
JAVA_TEST_CP:=./lib/junit4.jar:./lib/jsr305.jar:./lib/infer-annotations-1.4.jar:./src/java/src:./src/java/tests

ifeq (${OSTYPE}, cygwin)
JAVA_CP:=`cygpath -pw ${JAVA_CP}`
JAVA_SP:=`cygpath -pw ${JAVA_SP}` 
JAVA_TEST_CP:=`cygpath -pw ${JAVA_TEST_CP}`
endif

all: c c_test java java_test

c: transpile_all

c_test: c
	gcc -std=c99 -Werror -Wno-padded $(FILES) -lm -o a.out && ./a.out
	rm a.out

java: transpile_all src/java
	if [ ! -f lib/junit4.jar ]; then mkdir lib/; wget -O lib/junit4.jar http://search.maven.org/remotecontent?filepath=junit/junit/4.10/junit-4.10.jar; fi
	if [ ! -f lib/jsr305.jar ]; then mkdir lib/; wget -O lib/jsr305.jar http://search.maven.org/remotecontent?filepath=net/sourceforge/findbugs/jsr305/1.3.7/jsr305-1.3.7.jar; fi
	if [ ! -f lib/infer-annotations-1.4.jar ]; then mkdir lib/; wget -O lib/infer-annotations-1.4.jar https://github.com/facebook/buck/raw/master/third-party/java/infer-annotations/infer-annotations-1.4.jar; fi
	javac -cp ${JAVA_CP} -sourcepath ${JAVA_SP} src/java/tests/com/facebook/csslayout/*.java

java_test: java
	java -cp ${JAVA_TEST_CP} org.junit.runner.JUnitCore \
      com.facebook.csslayout.LayoutEngineTest \
      com.facebook.csslayout.LayoutCachingTest \
      com.facebook.csslayout.CSSNodeTest

transpile_all: ./src/transpile.js
	node ./src/transpile.js

debug:
	gcc -ggdb $(FILES) -lm -o a.out && lldb ./a.out
	rm a.out
