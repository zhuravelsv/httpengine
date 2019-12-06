using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace HttpEngine.Processing
{
    internal unsafe static class Extensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Diapason GetHeader(this Memory<char> memory)
        {
            var memorySpan = memory.Span;
            if (memorySpan.IsEmpty) return default;
            int length = memorySpan.Length;
            bool empty = true;
            char prevCharValue = memorySpan[0];
            for (int i = 1; i < length; i++)
            {
                char charValue = memorySpan[i];
                if(empty)
                {
                    if(!char.IsWhiteSpace(charValue))
                    {
                        empty = false;
                    }
                }
                if (charValue == '\n')
                {
                    if (prevCharValue == '\r')
                    {
                        return new Diapason(0, i);
                    }
                }
                prevCharValue = charValue;
            }
            if (empty) return default;
            return new Diapason(0, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Memory<char>[] Split(this Memory<char> memory, Span<char> separator)
        {
            return Split(memory, IndexOf(memory, separator));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Memory<char>[] Split(this Memory<char> memory, int[] positions)
        {
            int size = positions.Length;
            Memory<char>[] result = new Memory<char>[size];
            int prevPosition = 0;
            for(int i = 0; i < size; i++)
            {
                int currentPosition = positions[i];
                result[i] = memory.Slice(prevPosition, currentPosition - prevPosition);
                prevPosition = currentPosition;
            }
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CountOf(this Span<char> memorySpan, Span<char> content)
        {
            int dataLength = memorySpan.Length;
            int contentLength = content.Length;
            int forCount = dataLength - contentLength;
            int count = 0;
            fixed (char* dataPointer = memorySpan, contentPointer = content)
            {
                char* _dataPointer = dataPointer;
                char* _contentPointer = contentPointer;
                for (int i = 0; i < forCount; i++)
                {
                    _dataPointer = dataPointer + i;
                    bool equal = true;
                    for (int j = 0; j < contentLength; j++)
                    {
                        if (*(_dataPointer) != *(_contentPointer))
                        {
                            equal = false;
                            break;
                        }
                        _contentPointer++;
                        _dataPointer++;
                    }
                    if (equal)
                    {
                        i += contentLength - 1;
                        count++;
                    }
                }
                return count;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CountOf(this Memory<char> memory, Span<char> content)
        {
            return CountOf(memory.Span, content);
        }

        /// <summary>
        /// This method use stack (by stackalloc) for buffering indexes
        /// </summary>
        /// <param name="memorySpan"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int[] IndexOf(this Span<char> memorySpan, Span<char> content)
        {
            int dataLength = memorySpan.Length;
            int contentLength = content.Length;
            int forCount = dataLength - contentLength;
            int positionsStep = 20;
            int positionsCount = positionsStep;
            int positionId = 0;
            int arrayId = 0;
            int arraysCount = 5;
            fixed (char* dataPointer = memorySpan, contentPointer = content)
            {
                int** arrays = stackalloc int*[arraysCount];
                int* startPositions = stackalloc int[positionsCount];
                arrays[arrayId] = startPositions;
                int* positions = startPositions;
                char* _dataPointer = default;
                char* _contentPointer = contentPointer;
                for (int i = 0; i < forCount; i++)
                {
                    _dataPointer = dataPointer + i;
                    bool equal = true;
                    for (int j = 0; j < contentLength; j++)
                    {
                        if (*(_dataPointer + j) != *(_contentPointer + j))
                        {
                            equal = false;
                            break;
                        }
                    }
                    if (equal)
                    {
                        i += contentLength - 1;
                        if (positionId >= positionsStep)
                        {
                            int* newPosArray = stackalloc int[positionsStep];
                            arrayId++;
                            if (arrayId == arraysCount)
                            {
                                arraysCount *= 2;
                                int** newArrays = stackalloc int*[arraysCount];
                                for (int j = 0; j < arrayId; j++)
                                    newArrays[j] = arrays[j];
                                arrays = newArrays;
                            }
                            arrays[arrayId] = newPosArray;
                            positions = newPosArray;
                            positionsCount = positionsCount + positionsStep;
                            positionId = 0;
                        }
                        *(positions + positionId) = i;
                        positionId++;
                    }
                }
                positions = startPositions;
                int positionsLength = (positionsCount - positionsStep) + positionId;
                int[] result = new int[positionsLength];
                int arrayNumber = 0;
                for (int i = 0; i < positionsLength; i++)
                {
                    int actualPosition = i - positionsCount * arrayNumber;
                    if (actualPosition == positionsStep)
                    {
                        arrayNumber++;
                        positions = arrays[arrayNumber];
                        actualPosition = i - positionsStep * arrayNumber;
                    }
                    result[i] = positions[actualPosition];
                }
                return result;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int[] IndexOf(this Memory<char> memory, Span<char> content)
        {
            return IndexOf(memory.Span, content);
        }
    }
}
