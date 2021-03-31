using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trisemus
{
    class Program
    {
        static void Main(string[] args)
        {
            /*const string alpha = "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";*/
            const string alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            char[] alphabet = alpha.ToCharArray();
            
            // Пытаемся вычислить размерность таблицы
            Console.WriteLine("Quantity of alphabet symbols: " + alphabet.Length);
            int rows = 0, columns;
            bool isValidTable;
            do
            {
                Console.Write("Enter a columns quantity: ");
                isValidTable = int.TryParse(Console.ReadLine(), out columns) && columns > 1;
                if (!isValidTable)
                {
                    Console.WriteLine("You must enter a number greater than 1");
                }
                else
                {
                    rows = alphabet.Length / columns;
                    isValidTable &= rows > 1 && rows * columns == alphabet.Length;
                    if (!isValidTable)
                    {
                        Console.WriteLine("You must enter the number of columns so that the number of rows in the table is more than 1 and the table can contain all the characters in the alphabet");
                    }
                }
            }
            while (!isValidTable);

            // Пытаемся получить ключевое слово
            char[] keyWord;
            bool isValidKeyWord;
            do
            {
                Console.Write("Enter key word: ");
                keyWord = Console.ReadLine().ToUpper().Distinct().ToArray();
                Console.WriteLine("KEY WORD: " + new string(keyWord));
                isValidKeyWord = keyWord.Length > 0 && keyWord.Length <= alphabet.Length;
                if (!isValidKeyWord)
                {
                    Console.WriteLine("The keyword cannot be an empty string or contain a number of unique characters larger than the size of the alphabet");
                }
                else
                {
                    isValidKeyWord &= !keyWord.Except(alphabet).Any();
                    if (!isValidKeyWord)
                    {
                        Console.WriteLine("Keyword cannot contain characters that are not in the alphabet");
                    }
                }
            }
            while (!isValidKeyWord);

            // create table
            var table = new char[rows, columns];

            // enter a key word
            for (var i = 0; i < keyWord.Length; i++)
            {
                table[i / columns, i % columns] = keyWord[i];
            }

            // delete unique symbols of word from alphabet
            alphabet = alphabet.Except(keyWord).ToArray();
            Console.WriteLine("alphabet without literals from keyWord: " + new string(alphabet));

            string lastChar = (new string(keyWord)).Substring((new string(keyWord)).Length - 1); //take a last character from "keyWord" string
            string newChar = "";   
            
            for(int i = 0; i<alpha.Length; i++)
            {
                if(alpha[i].ToString() == lastChar)
                {
                    if (!keyWord.Contains(alpha[i+1]))
                    {
                        newChar = alpha[i + 1].ToString();  //have a next letter
                    }
                }
            }



            Console.WriteLine("NEW CHAR: " + newChar);

            int lastIndex = (new string(alphabet)).IndexOf(newChar);    //get an index of "lastChar" into "alpha" string

            Console.WriteLine("Index of " + newChar + " in alphabet: " + lastIndex);
            // Enter alphabet
            for (var i = 0; i < alphabet.Length; i++)
            {
                int position = i + keyWord.Length;
                table[position / columns, position % columns] = alphabet[(i+lastIndex)%alphabet.Length];
            }


            Console.WriteLine("Table: ");
            Console.WriteLine("-------------");
            for (int i = 0; i<rows; i++)
            {
                for(int j = 0; j<columns; j++)
                {
                    Console.Write(table[i, j] + " ");
                }
                Console.WriteLine();
            }
            Console.WriteLine("\n-------------");

            // get a word for cipher
            string message = "";
            bool isValidMessage;

                try
                {
                    using (StreamReader sr = new StreamReader("E:\\Study\\3_course\\2_Semester\\Cryptography\\Labs\\Crypto\\lab04(Подстановочные_Шифры)\\Trisemus\\file.txt"))
                    {
                        message = (((sr.ReadToEnd()).ToUpper()).Replace(" ", ""));
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                Console.Write("MESSAGE: " + message);

            // allocate memory for an encrypted word

            Stopwatch time_encrypt = new Stopwatch();
            Stopwatch time_decrypt = new Stopwatch();

            string read_text = "";
            foreach (var i in message)
            {
                if (alpha.Contains(i))
                {
                    read_text += i;
                }
            }

            Console.WriteLine("\n\nRead text from file: " + read_text + "\n\n");    //message for encription
            var result = new char[read_text.Length];
            //....................................................................................................................................................

            // encrypt the word
            time_encrypt.Start();
            for (var k = 0; k < read_text.Length; k++)
            {
                    char symbol = read_text[k];
                    // Try to find symbol in the table
                    for (var i = 0; i < rows; i++)
                    {
                        for (var j = 0; j < columns; j++)
                        {
                            if (symbol == table[i, j])
                            {
                                symbol = table[(i + 1) % rows, j];
                                i = rows; // end cycle by rows
                                break; // end cycle by columns
                            }
                        }
                    }
                    // record the found character
                    result[k] = symbol;
            }
            time_encrypt.Stop();
            // Write an encrypted word
            string encrypted_word = new string(result);
            Console.WriteLine("Encrypted word: " + encrypted_word);
            Console.WriteLine("Encryption runtime: " + time_encrypt.Elapsed.TotalMilliseconds.ToString() + " milliseconds");

            try
            {
                using (StreamWriter sw = new StreamWriter("E:\\Study\\3_course\\2_Semester\\Cryptography\\Labs\\Crypto\\lab04(Подстановочные_Шифры)\\Trisemus\\encrypt.txt", false, System.Text.Encoding.UTF8))
                {
                    sw.Write(encrypted_word);
                    Console.WriteLine("\nEncrypted message written to file");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            try
            {
                using (StreamReader sr = new StreamReader("E:\\Study\\3_course\\2_Semester\\Cryptography\\Labs\\Crypto\\lab04(Подстановочные_Шифры)\\Trisemus\\encrypt.txt"))
                {
                    message = (((sr.ReadToEnd()).ToUpper()).Replace(" ", ""));
                    Console.WriteLine("Encrypted message read from file");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            // allocate memory for an decrypted word
            var decrypted_word = new char[message.Length];

            // decrypt the word
            time_decrypt.Start();
            for (var k = 0; k < message.Length; k++)
            {
                char symbol = message[k];
                // Пытаемся найти символ в таблице
                for (var i = 0; i < rows; i++)
                {
                    for (var j = 0; j < columns; j++)
                    {
                        if (symbol == table[i, j])
                        {
                            if(i == 0)
                            {
                                symbol = table[rows-1, j];
                                i = rows;
                                break;
                            }
                            else
                            {
                                symbol = table[i-1, j]; 
                                i = rows; 
                                break;
                            }
                        }
                    }
                }
                // record the found character
                decrypted_word[k] = symbol;
            }
            time_decrypt.Stop();
            // Write an decrypted word
            string deResult = new string(decrypted_word);
            Console.WriteLine("\nDecrypted word: " + deResult);
            Console.WriteLine("Decryption runtime: " + (time_decrypt.Elapsed.TotalMilliseconds).ToString() + " milliseconds");

            /*try
            {
                using (StreamWriter sw = new StreamWriter("E:\\3_course\\2_Semester\\Cryptography\\Labs\\lab04\\Trisemus\\file.txt", false, System.Text.Encoding.UTF8))
                {
                    sw.Write(deResult);
                    Console.WriteLine("\nDecrypted message written to file");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }*/

            Console.WriteLine("\n-- Character Frequency for an Encrypted Word --\n");
            foreach (var e in alphabet)
            {
                double prc = (double)((new string(result)).ToUpper().Count(s => s.ToString() == e.ToString())) / (double)((new string(result)).ToUpper().Count(s => alphabet.Any(p => p.ToString() == s.ToString())));
                Console.WriteLine(e + ": " + prc); //Выводим процент содержания каждой буквы в текст
            }

            Console.WriteLine("\n-- Character Frequency for an Decrypted Word --\n");
            foreach (var e in alphabet)
            {
                double prc = (double)((new string(decrypted_word)).ToUpper().Count(s => s.ToString() == e.ToString())) / (double)((new string(decrypted_word)).ToUpper().Count(s => alphabet.Any(p => p.ToString() == s.ToString())));
                Console.WriteLine(e + ": " + prc); //Выводим процент содержания каждой буквы в текст
            }

            Console.WriteLine();
        }
    }
}
