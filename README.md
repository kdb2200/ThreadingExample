### Threading TLDR

Threads allow for multiple processes to be running "at the same time" (Synchronously). They are helpful for event waiting (e.g. waiting for a keyboard press) or parallelizable problems.

### Threads with respect to CPUs and Cores - The Operating System Overlord

Typically, the operating system manages all the active threads and processes. From its perspective, there are multiple processes that need to run so it will allocate the available computational resources accordingly.

A CPU is a computational resource that may or may not contain multiple cores. Each core can run a process (e.g. some code) and the process is designated by the operating system. In the example of one core and one process, the process can be run on the core to completion. However, if we have one core and multiple processes (we'll say two for now), we run into an issue.

### How do we know what process to run?

One solution would be to run one process to completion and then run the second process to completion. Both would finish and the end result would be the same. The big issue with this is: what if one of those processes is your mouse? Would you want your mouse to hang in place while some other process has to finish what it's doing? Probably not, which is why this solution isn't going to work for us.

An alternative is to give some time for the first process to run, and then give some time for the second process to run. Keep switching back and forth allocating some time on the CPU core until the processes finish. In the mouse example again, we're never waiting out the full other process before our mouse get's a chance to react. Our mouse will be much more responsive since we're periodically giving resources to it.

### Parallel Programming

Now that we talked about one core and two processes, let's flip it. Two cores and one process. Parallel programming lets us use multiple CPU cores by splitting up our processes into parallelizable segments. One example of this is adding up the sum of multiple lists of numbers.

Consider the following two lists:

L1: [2, 5, 7]

L2: [3, 4, 6]

And a function that sums all the numbers of a list:

Sum(L) => Sum of the list L

Without parallelizing this problem we might find the sum of each list by doing the following:

Run Sum(L1) => 14

Then Run Sum(L2) => 13

But by saying we want to run Sum(L1) and Sum(L2) synchronously (In C# this is by using the threads class), our OS will know to allocate time on *each* core to *each* Sum process.

Run Sum(L1) (OS might run it on core 1) => 14

And Run Sum(L2) (OS might run it on core 2) => 13

These processes run at the same time and we would expect the program to finish about twice as fast.
