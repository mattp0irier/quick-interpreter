# Quick
#### *Swiftly make well-formatted tests* 
##### Matt Poirier and Trevor Russell

---

This project is for CS 403 Spring 2022. In this project, we designed our own programming language to fill a real purpose. We designed Quick to allow for easy test creation and generation.

---

### Purpose
Almost everyone has taken a class where the teacher or professor gave tests that were not formatted well. While tools like Microsoft Word are great for many things, the immense feature set can lead to it being difficult to format some seemingly simple bodies of text.
Quick is our idea of a language that could allow users to easily create tests in a variety of ways while maintaining good formatting. By allowing users to create questions of various types and store them in banks by topic, users can create tests that coalesce different topics. Users can then generate several forms of a test that shuffle the question order between versions and output the test as an `.rtf` file. A corresponding key is also generated for each form of the test.
In all, we believe these features can simplify the test creation process for educators by eliminating the worry of formatting.

---

### Grammar
Below is a grammar written in EBNF form as specified by Wikipedia. It can also be found in the `grammar.txt` file located in the source folder.

```
STRING = ? a terminal string with a user-provided value ?;
NUMBER = ? a terminal integer provided by the user ?;

TEST_NAME = STRING;
TEST_TITLE = STRING;
BANK_NAME = STRING;
PROBLEM = STRING;
ANSWER = STRING;
OTHER_ANSWERS = STRING;
SOLUTION = NUMBER | 'T' | 'F';

NEW_QUESTION = 'mc', PROBLEM, ANSWER, {OTHER_ANSWERS}, NUMBER
	     |'tf', PROBLEM, 'T' | 'tf', PROBLEM, 'F'
             |'sa', PROBLEM
             |'fr', PROBLEM, NUMBER;

MORE_QUESTIONS = NEW_QUESTION, ',';

QUESTION_BLOCK = '{', [ { MORE_QUESTIONS } , NEW_QUESTION ], '}';

generate = 'generate', TEST_NAME, TEST_TITLE, [NUMBER], ['shuffle'], ';';
bank = 'bank', BANK_NAME, [QUESTION_BLOCK], ';';
test = 'test', TEST_NAME, [QUESTION_BLOCK], ';'; 
question = 'question', NEW_QUESTION, ';' | 'question', QUESTION_BLOCK, ';';
print = 'print', BANK_NAME, ';' | 'print', TEST_NAME, ';';
delete = 'delete', BANK_NAME, '[', NUMBER, ']', ';';
set = 'set', BANK_NAME, '[', NUMBER, ']', SOLUTION, ';';
```

---

### Syntax

##### Questions
There are four types of question: multiple choice, true/false, short answer, and free response.

Multiple choice questions can have up to 4 answers displayed. The correct answer is set by a number at the end of the question indicating which choice is correct (this value starts at 1).
```
question bank1 mc "This is the question text" "1st answer" "2nd answer" "3rd answer" "4th answer" 4;
question bank1 mc "This question only has 2 answers" "The correct one" "The incorrect one" 1;
```

True/false questions follow similar syntax to multiple choice questions, with the correct answer being set by either a T or F.

```
question bank1 tf "This statement is false" F;
```

Short answer questions simply ask for the question. Quick provides a set amount of space to answer these questions when generating tests.
```
question bank1 sa "This is a short answer question";
```

Free response questions ask for the question and the number of lines to provide for writing on the test.
```
question bank1 fr "This is a free response question" 12;
```

##### Banks
Banks serve as collections of questions. Banks can be created as shown below.
```
bank bank1;
```

Banks can also be created with questions at the same time. Each question is separated by a comma.
```
bank Bank1 {
	tf "This is a True/False question; the answer is False." T,
	fr "This is a free response question." 12
};
```

Questions can be removed from a test bank using the index number of the question, which can be discovered by printing the test bank.
```
delete bank1[3];
```

The answer for a multiple choice or true/false question already in a bank can be modified using a similar command.

```
set_ans bank1[2] F;
```

##### Tests
Tests compile questions from banks for a single exam. Tests can either pull from one bank or multiple banks, as shown in the commands below.
```
test test1 {Bank1, Bank2};
test test2 Bank2;
```

##### Generation
Once a test is created, it can be outputted using the `generate` command. A plaintext name to be displayed as the header of the output file, the number of versions of the test to generate, and if the question order should be shuffled is specified as flags for the command.
```
generate test2 "Example Exam: Test 2";
generate test1 "Example Exam: Test 1" 4 shuffle;
```

##### Miscellaneous
When using the prompt, a bank or test can be displayed by using the `print` command.
```
print bank1;
print test1;
```

When using a source file, comments in the file can be indicated by a `/`.
```
/ This is a comment above a print statement.
print finalExam;
```

---

### Compilation Instructions
This project was built and compiled inside of Visual Studio 2022 using .NET 6.0.

The command used to compile the project is `msbuild quick-interpreter.csproj`.

To run in User Input mode:
`./bin/Debug/net6.0/quick-interpreter`

To run with an input file:
`./bin/Debug/net6.0/quick-interpreter filename`

---

### Examples
Various tests were run on the interpreter in order to verify that Quick could generate tests in all of the manners implemented in the language. Included test files are:

- `quickTest`: includes 2 test banks with each type of question, test editing and deleting questions, and generates different forms of the tests

The outputs of our tests are stored in the files with the `.rtf` extension.

**NOTE:** We opened our tests in Microsoft Word. Different editors render RTF files differently, so it is difficult to ensure correct formatting for all RTF editors. Outputting tests in a more consistent file format (such as PDF) would prevent this issue (see Future Improvements).

---

### Future Improvements
We recognize that this language could be improved in many ways given more time and energy. Listed below are some of our ideas to improve the language:

- Add matching question type
- Allow for questions to be paired (i.e. a true/false question followed by an explanation free response question)
- Allow for shuffling of multiple choice answers within a question
- Include *x* questions from a bank on a test
- Implement adding images
- Save in other document formats (PDF, DOCX, etc.)
- Offline storage of banks and tests

---

### References
Robert Nystrom's JLox interpreter from his textbook *Crafting Interpreters* was used as a guide to create the interpreter for Quick. Referencing the Lox interpreter we implemented for our last project provided us with a clear path to an interpreter for this project.

In addition, the [.NET RTF Writer Library in C# Files](https://sourceforge.net/projects/netrtfwriter/files/netrtfwriter-1.0.0/) was used in order to write to RTF files. While this RTF library is relatively outdated (given it uses version RTF version 1.0), it was the simplest and, perhaps more notably, the only free library we could find. In fact, in order to implement it, all image features had to be commented out in order to get the code to compile. Certainly if Quick was to be implemented for release, a commercial RTF library would be purchased and implemented. This would also allow for generation of tests using other formats.