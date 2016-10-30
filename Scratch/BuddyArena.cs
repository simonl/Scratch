using System;
using System.Collections.Generic;

namespace Scratch
{
    public static class BuddyArena
    {
        private const byte BLOCK_FLAG = 128;
        private const byte SIZE_MASK = 128-1;

        private static bool IsBlockFree(byte[] memory, uint pointer)
        {
            if (pointer == memory.Length)
            {
                return false;
            }

            var flag = memory[pointer] & BLOCK_FLAG;

            return flag == 0;
        }

        private static uint BlockSize(byte[] memory, uint pointer)
        {
            if (pointer == memory.Length)
            {
                return (uint) memory.Length;
            }

            var power = memory[pointer] & SIZE_MASK;

            return (uint) (1 << power);
        }

        public static byte[] CreateArena(byte power)
        {
            if (!(power < BLOCK_FLAG))
            {
                throw new ArgumentException();
            }

            var size = (uint) (1 << power);

            var memory = new byte[size];

            memory[0] = power;

            return memory;
        }

        public static uint Malloc(byte[] memory, uint size)
        {
            var pointer = FindBlock(memory, size);
            
            while (BlockSize(memory, pointer) != size)
            {
                SplitBlock(memory, pointer);
            }

            memory[pointer] ^= BLOCK_FLAG;

            return pointer;
        }

        private static void SplitBlock(byte[] memory, uint pointer)
        {
            memory[pointer]--;
            memory[pointer + BlockSize(memory, pointer)] = memory[pointer];
        }

        private static uint FindBlock(byte[] memory, uint size)
        {
            if (size == 0 || memory.Length < size)
            {
                throw new ArgumentException();
            }
            
            foreach (var pointer in Traverse(memory))
            {
                if (!IsBlockFree(memory, pointer))
                {
                    continue;
                }

                if (BlockSize(memory, pointer) < size)
                {
                    continue;
                }

                return pointer;
            }

            throw new InsufficientMemoryException();
        }
        
        private static IEnumerable<uint> Traverse(byte[] memory)
        {
            var pointer = 0U;
            while (pointer != memory.Length)
            {
                yield return pointer;

                pointer += BlockSize(memory, pointer);
            }
        }

        public static void FreeBlock(byte[] memory, uint pointer)
        {
            if (IsBlockFree(memory, pointer))
            {
                throw new ArgumentException();
            }

            memory[pointer] ^= BLOCK_FLAG;

            MergeBlock(memory, pointer);
        }

        private static void MergeBlock(byte[] memory, uint pointer)
        {
            while (true)
            {
                var buddy = Buddy(memory, pointer);

                if (BlockSize(memory, buddy) != BlockSize(memory, pointer))
                {
                    return;
                }

                if (!IsBlockFree(memory, buddy))
                {
                    return;
                }

                pointer = Parent(memory, pointer);

                memory[pointer]++;
            }
        }

        private static uint Buddy(byte[] memory, uint pointer)
        {
            return pointer ^ BlockSize(memory, pointer);
        }

        private static uint Parent(byte[] memory, uint pointer)
        {
            return pointer & ~BlockSize(memory, pointer);
        }
        
        
        // Buddy(pointer) != pointer
        // Size(Buddy(pointer)) <= Size(pointer)
        // Parent(Parent(pointer)) == Parent(pointer)
        // Size(pointer) == Size(Buddy(pointer)) -> Buddy(Buddy(pointer)) == pointer
        // Size(pointer) == Size(Buddy(pointer)) -> Parent(Buddy(pointer)) == Parent(pointer)
        // Size(pointer) == Size(Buddy(pointer)) -> Parent(pointer) == pointer || Parent(pointer) == Buddy(pointer)
        // Size(pointer) == Size(Buddy(pointer)) -> !(Free(pointer) && Free(Buddy(pointer))
        
        private static void BuddyIsNotSelf(byte[] memory, uint pointer)
        {
            if (Buddy(memory, pointer) == pointer)
            {
                throw new InvalidProgramException();
            }
        }

        private static void BuddyCannotBeLarge(byte[] memory, uint pointer)
        {
            if (BlockSize(memory, pointer) < BlockSize(memory, Buddy(memory, pointer)))
            {
                throw new InvalidProgramException();
            }
        }

        private static void ParentIsIdempotent(byte[] memory, uint pointer)
        {
            if (Parent(memory, Parent(memory, pointer)) != Parent(memory, pointer))
            {
                throw new InvalidProgramException();
            }
        }

        private static void BuddyIsInvolution(byte[] memory, uint pointer)
        {
            if (BlockSize(memory, pointer) == BlockSize(memory, Buddy(memory, pointer)))
            {
                if (Buddy(memory, Buddy(memory, pointer)) != pointer)
                {
                    throw new InvalidProgramException();
                }
            }
        }

        private static void BuddiesHaveSameParent(byte[] memory, uint pointer)
        {
            if (BlockSize(memory, pointer) == BlockSize(memory, Buddy(memory, pointer)))
            {
                if (Parent(memory, pointer) != Parent(memory, Buddy(memory, pointer)))
                {
                    throw new InvalidProgramException();
                }
            }
        }

        private static void ParentIsSelfOrBuddy(byte[] memory, uint pointer)
        {
            if (BlockSize(memory, pointer) == BlockSize(memory, Buddy(memory, pointer)))
            {
                if (Parent(memory, pointer) != pointer && Parent(memory, pointer) != Buddy(memory, pointer))
                {
                    throw new InvalidProgramException();
                }
            }
        }

        private static void BuddiesAreNotFree(byte[] memory, uint pointer)
        {
            if (BlockSize(memory, pointer) == BlockSize(memory, Buddy(memory, pointer)))
            {
                if (IsBlockFree(memory, pointer) && IsBlockFree(memory, Buddy(memory, pointer)))
                {
                    throw new InvalidProgramException();
                }
            }
        }

        public static uint SpareCapacity(byte[] memory)
        {
            uint capacity = 0;

            foreach (var pointer in Traverse(memory))
            {
                if (IsBlockFree(memory, pointer))
                {
                    capacity += BlockSize(memory, pointer);
                }
            }

            return capacity;
        }

        public static void Main(string[] args)
        {
            byte size = 5;
            var memory = CreateArena(size);

            while (true)
            {
                try
                {
                    
                    Console.WriteLine(SpareCapacity(memory) + "/" + memory.Length);

                    foreach (var pointer in Traverse(memory))
                    {
                        BuddyIsNotSelf(memory, pointer);
                        BuddyCannotBeLarge(memory, pointer);
                        ParentIsIdempotent(memory, pointer);
                        BuddyIsInvolution(memory, pointer);
                        BuddiesHaveSameParent(memory, pointer);
                        ParentIsSelfOrBuddy(memory, pointer);
                        BuddiesAreNotFree(memory, pointer);

                        Console.Write("| " + BlockSize(memory, pointer) + (IsBlockFree(memory, pointer) ? "?" : "!") + " |");
                    }


                    Console.WriteLine();

                    Console.Write("> ");
                    var command = Console.ReadLine().Split(' ');
                    
                    switch (command[0])
                    {
                        case "malloc":
                        {
                            var pointer = uint.Parse(command[1]);

                            Console.WriteLine(Malloc(memory, pointer));

                            break;
                        }
                        case "free":
                        {
                            var pointer = uint.Parse(command[1]);

                            FreeBlock(memory, pointer);

                            break;
                        }
                        case "reset":
                        {
                            size = byte.Parse(command[1]);
                            memory = CreateArena(size);

                            break;
                        }
                        case "collect":
                        {
                            var toSpace = CreateArena(size);

                            foreach (var pointer in Traverse(memory))
                            {
                                if (IsBlockFree(memory, pointer))
                                {
                                    continue;
                                }

                                Malloc(toSpace, BlockSize(memory, pointer));
                            }

                            memory = toSpace;

                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            Console.ReadLine();
        }

    }
}