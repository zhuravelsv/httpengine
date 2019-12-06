using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("HttpEngine.UnitTests")]
namespace HttpEngine
{
    internal static unsafe class MemoryHelpers
    {
        private static readonly long _stringVmt;
        private static readonly long _charArrayVmt;

        static MemoryHelpers()
        {
            _stringVmt = typeof(string).TypeHandle.Value.ToInt64();
            char[] charArray = new char[0];
            _charArrayVmt = charArray.GetType().TypeHandle.Value.ToInt64();
        }

        /// <summary>
        /// Very very very very unsafe method (the most unsafe method of the most unsafe methods in the world and maybe in our universe 
        /// (dont use it in airplanes - in some cases may crash CLR and your airplane by uncatchable ExecutingEngineException)):
        /// makes string from bytes array by editing pointer to VMT (only for UTF (Unicode) content)
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string MakeString(byte[] bytes)
        {
            fixed (void* pointer = bytes) //pin array
            {
                long stringVmt = _stringVmt; //get VMT address for string
                var typedReference = __makeref(bytes); //get TypedReference for bytes array
                var typedReferencePointer = (IntPtr*)&typedReference; //get pointer to typed reference
                IntPtr valuePtr = *(IntPtr*)(typedReferencePointer[0]); //get pointer to bytes array in heap
                (typedReferencePointer)[1] = new IntPtr(stringVmt); //change type pointer (VMT) from bytes array to string
                long addr = (valuePtr).ToInt64(); //get address to bytes array in memory
                *((long*)addr) = stringVmt; //change pointer to VMT from bytes aray to string
                *((int*)addr + 2) = bytes.Length / 2 + 2; //set new size (2 bytes per char)
                string str = __refvalue(typedReference, string) as string; //get string from edited typed reference
                return str; //ret
            }
        }

        public static char[] MakeCharArray(byte[] bytes)
        {
            int offset = 8 + 4 + 4 + bytes.Length;
            fixed(void * pointer = bytes) //pin array
            {
                long charArrayVmt = _charArrayVmt;
                var typedReference = __makeref(bytes); //get TypedReference for bytes array
                var typedReferencePointer = (IntPtr*)&typedReference; //get pointer to typed reference
                IntPtr valuePtr = *(IntPtr*)(typedReferencePointer[0]); //get pointer to bytes array in heap
                (typedReferencePointer)[1] = new IntPtr(charArrayVmt); //change type pointer (VMT) from bytes array to char array
                long addr = (valuePtr).ToInt64(); //get address to bytes array in memory
                *((long*)addr) = charArrayVmt; //change pointer to VMT from bytes aray to char array
                *((int*)addr + 2) = bytes.Length / 2; //set new size (2 bytes per char)
                char[] result = __refvalue(typedReference, char[]) as char[]; //get char array from edited typed reference
                return result; //ret
            }
        }
    }
}
