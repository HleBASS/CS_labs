using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab2
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter first number:");
            int num1 = int.Parse(Console.ReadLine());
            Console.Write("Enter second number:");
            int num2 = int.Parse(Console.ReadLine());
            //Multiply(num1, num2);
            //Division(num1, num2);
            Float_Add(num1, num2);


        }


        private static void Multiply(int multiplicand, int multiplier)
        {
            Int64 product = 0;
            int counter = 1;
            for (int i = 0; i < 32; i++)
            {
                Console.WriteLine($"\n{counter++}| Product: " + ConvertNumberToBinary(product, true));
                Console.WriteLine($"{counter++}| Multiplicand: " + ConvertNumberToBinary(multiplicand, false));
                Console.WriteLine($"{counter++}| Multiplier: " + ConvertNumberToBinary(multiplier, false));
                Console.WriteLine();
                if ((multiplier & 1) == 1)
                {
                    product += multiplicand;
                    Console.WriteLine($"{counter++}| Product + multiplicand: " + ConvertNumberToBinary(product, true));
                }
                multiplicand <<= 1;
                Console.WriteLine($"{counter++}| Multiplicand leftshifted: " + ConvertNumberToBinary(multiplicand, false));
                multiplier >>= 1;
                Console.WriteLine($"{counter++}| Multiplier rightshifted: " + ConvertNumberToBinary(multiplier, false) + "\n-----------");
            }
            Console.WriteLine($"{counter++}| result: " + product.ToString() + "\nBinary: " + ConvertNumberToBinary(product, true) );
        }


        private static void Division(int _dividend, int _divisor)
        {
            Int64 divisor = _divisor;
            Int64 remaiderAndQuotient = _dividend;
            int c = 1;
            bool setLSBtoONE = false;
            divisor <<= 32;
            for (int i = 0; i < 33; i++)
            {
                if (divisor <= remaiderAndQuotient)
                {
                    remaiderAndQuotient -= divisor;
                    setLSBtoONE = true;
                }
                remaiderAndQuotient <<= 1;
                if (setLSBtoONE)
                {
                    setLSBtoONE = false;
                    remaiderAndQuotient |= 1;
                }

                Console.WriteLine($"\n{c++}| Divisor: " + ConvertNumberToBinary(divisor, true) + "\nRemainder + quotient: " + ConvertNumberToBinary(remaiderAndQuotient, true) + "\n----------------------------");
            }
            Int64 quotient = remaiderAndQuotient & ((long)Math.Pow(2, 33) - 1);
            Int64 remainder = remaiderAndQuotient >> 33;


            Console.WriteLine($"{c++}| Quotient: " + ConvertNumberToBinary(quotient, false) +
            "[" + quotient + "]");

            Console.WriteLine($"{c++}| Remainder: " + ConvertNumberToBinary(remainder,false) +
            "[" + remainder + "]");

        }


        public static void Float_Add(float First, float Second)
        {
            int bias = (int)(Math.Pow(2, 7) - 1);
            bool is_adding = First * Second >= 0;
            if (Math.Abs(Second) > Math.Abs(First))
            {
                float temp;
                temp = First;
                First = Second;
                Second = temp;
            }

            Console.WriteLine($"adding {First} (a) to {Second} (b)");
            int a_sign_bit = First < 0 ? 1 : 0,
                b_sign_bit = Second < 0 ? 1 : 0;
            First = Math.Abs(First);
            Second = Math.Abs(Second);

            int a_int_bits = (int)First,
                b_int_bits = (int)Second;
            First -= a_int_bits;
            Second -= b_int_bits;

            FloatBits(First, out int a_float_bits);
            FloatBits(Second, out int b_float_bits);


            Console.WriteLine($"Convert \"a\" to binary:\n{ResultToBinaryString(a_sign_bit, 0, a_float_bits)}");
            Console.WriteLine($"Convert \"b\" to binary:\n{ResultToBinaryString(b_sign_bit, 0, b_float_bits)}");

            Int16 exp_a = Normalize(a_int_bits, ref a_float_bits),
                exp_b = Normalize(b_int_bits, ref b_float_bits);

            byte exponent_a = (byte)(exp_a + bias),
                exponent_b = (byte)(exp_b + bias);

            string a_float_bits_string = ResultToBinaryString(a_sign_bit, exponent_a, a_float_bits);
            Console.WriteLine($"Normalize \"a\":\n{a_float_bits_string}");
            Console.WriteLine($"Normalize \"b\":\n{ResultToBinaryString(b_sign_bit, exponent_b, b_float_bits)}");

            b_float_bits >>= exp_a - exp_b;

            string b_float_bits_string = ResultToBinaryString(b_sign_bit, exponent_b, b_float_bits);
            Console.WriteLine("Left carry \"b\" on {0}:\n{1}", exp_a - exp_b, b_float_bits_string);
            Console.WriteLine("Add \"a\" to \"b\":\n {0}\n+{1}", a_float_bits_string, b_float_bits_string);

            Int32 result = is_adding ? a_float_bits + b_float_bits : a_float_bits - b_float_bits;
            NormilizeResult(ref result, ref exp_a, is_adding);
            exponent_a = (byte)(exp_a + bias);

            Console.WriteLine("Product :\ndecimal: {0}\nbinary: {1}",
                ConvertToDecimal(exp_a, result, a_sign_bit), ResultToBinaryString(a_sign_bit, exponent_a, result));
        }
        private static void FloatBits(float value, out Int32 float_bits)
        {
            const int amount_of_mantisa_bits = 23;
            int i = 0;

            float_bits = 0;
            while (value != 0 && i < 22)
            {
                value *= 2;
                if (value >= 1)
                {
                    float_bits |= 1;
                    value -= 1;
                }
                float_bits <<= 1;
                ++i;
            }
            float_bits <<= amount_of_mantisa_bits - i - 1;
            Console.WriteLine();
        }
        private static Int16 Normalize(int value, ref Int32 float_bits)
        {
            Int16 exp = 0;
            Int32 hidden_one = 1 << 23;

            if (value > 0)
            {
                while (value > 1)
                {
                    ++exp;
                    float_bits >>= 1;
                    float_bits |= (value & 1) << 22;
                    value >>= 1;
                }
                float_bits |= hidden_one;
            }

            return exp;
        }
        private static void NormilizeResult(ref Int32 result, ref Int16 exp, bool is_adding)
        {
            Int32 hidden_one = 1 << 23;

            if ((result & hidden_one) == hidden_one)
            {
                ++exp;
                result >>= 1;
                return;
            }

            if (is_adding)
                do
                {
                    ++exp;
                    result >>= 1;
                } while ((result & hidden_one) != hidden_one);
            else
                do
                {
                    --exp;
                    result <<= 1;
                } while ((result & hidden_one) != hidden_one);
        }

        private static string ResultToBinaryString(int sign_bit, byte exponent, Int32 result)
        {
            string result_string = sign_bit + " . ";
            for (int i = 7; i >= 0; --i)
                result_string += exponent >> i & 1;
            result_string += " . ";
            for (int i = 22; i >= 0; --i)
                result_string += result >> i & 1;

            return result_string;
        }
        private static float ConvertToDecimal(Int16 exp_a, Int32 mantissa, int sign_bit)
        {
            float result = 0,
                multiplier = (float)Math.Pow(2, exp_a);


            for (int i = 23; i >= 0; --i, multiplier /= 2)
                result += multiplier * (mantissa >> i & 1);
            if (sign_bit == 1)
                result = -result;
            return result;
        }





        private static string ConvertNumberToBinary(Int64 number, bool big)
        {
            string bits = "";
            int max = (big) ? 64 : 32;
            for (int i = 0; i < max; i++)
            {
                bits = ((i + 1) % 4 == 0 ? " " : "") + (number & 1) + bits;
                number >>= 1;
            }
            return bits;
        }
    }
}
