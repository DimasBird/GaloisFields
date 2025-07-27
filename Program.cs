using Exceptions;
using System;
using System.Windows.Forms;
using System.Text;
namespace Exceptions
{
    public class IncompatibleGaloisFieldMembers : Exception
    {
        public IncompatibleGaloisFieldMembers() : base("Несоответствие n или p в полях Галуа.") { }
    }
    public class CurrentMemberIsNotForming : Exception
    {
        public CurrentMemberIsNotForming() : base("Выбранный элемент не является образующим данной группы.") { }
    }
    public class WrongIrreduciblePolynomial : Exception
    {
        public WrongIrreduciblePolynomial() : base("Данный многочлен не является неприводимым.") { }
    }
    public class IncompatibleCharAndField : Exception
    {
        public IncompatibleCharAndField() : base("Значение char не влезает в выбранное поле.") { }
    }
    public class NonexistingCharInDict : Exception
    {
        public NonexistingCharInDict() : base("Данного символа нет в выбранном алфавите.") { }
    }
    public class WrongDictForField : Exception
    {
        public WrongDictForField() : base("У поля и словаря разные размеры.") { }
    }
    public class WrongInput : Exception
    {
        public WrongInput() : base("Неправильный ввод данных.") { }
    }
    public class NoValidNumbersFound : Exception
    {
        public NoValidNumbersFound() : base("Не найдены параметры поля Галуа.") { }
    }
}

namespace Galois_Field
{
    public class Galois_Field
    {
        private int p;
        private int n;
        private int fx;

        private int forming_element;
        private int member;

        public Galois_Field(int p, int n, int member)
        {
            this.p = p;
            this.n = n;

            this.fx = find_fx();
            this.forming_element = find_forming_element();

            this.member = Numbers.Number_Operations.normalizer(member, fx, n, p);
        }
        public Galois_Field(int p, int n, int member, int fx)
        {
            this.p = p;
            this.n = n;

            if (check_if_fx_irreducible(fx)) { this.fx = fx; }
            else { throw new Exceptions.WrongIrreduciblePolynomial(); }

            this.forming_element = find_forming_element();

            this.member = Numbers.Number_Operations.normalizer(member, fx, n, p);
        }
        public Galois_Field(int p, int n, int member, int fx, int forming_element)
        {
            this.p = p;
            this.n = n;

            if (check_if_fx_irreducible(fx)) { this.fx = fx; }
            else { throw new Exceptions.WrongIrreduciblePolynomial(); }

            if (check_if_forming(forming_element)) { this.forming_element = forming_element; }
            else { throw new Exceptions.CurrentMemberIsNotForming(); }

            this.member = Numbers.Number_Operations.normalizer(member, fx, n, p);
        }

        public int get_p() { return p; }
        public int get_n() { return n; }
        public int get_fx() { return fx; }
        public int get_member() { return member; }
        public int get_forming_element() { return forming_element; }
        public void set_member(int member) { this.member = member; }

        public void set_forming_element(int suggested_forming_member)
        {
            if (check_if_forming(member) == false)
            {
                throw new Exceptions.CurrentMemberIsNotForming();
            }
            this.forming_element = suggested_forming_member;
        }

        public bool check_if_fx_irreducible(int fx)
        {
            if (fx < Numbers.Number_Operations.pow(p, n) + 1) { return false; }
            for (int member_a = p; member_a <= Numbers.Number_Operations.pow(p, n) - 1; member_a++)
            {
                for (int member_b = member_a; member_b <= Numbers.Number_Operations.pow(p, n) - 1; member_b++)
                {
                    if (fx == Numbers.Number_Operations.mul(member_a, member_b, p))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        private int find_fx()
        {
            int max_member = Numbers.Number_Operations.pow(p, n + 1);
            int min_member = Numbers.Number_Operations.pow(p, n);
            int out_fx = -1;

            for (int current_fx = min_member; current_fx < max_member; current_fx++)
            {
                bool fx_found = true;

                for (int member_a = p; member_a <= Numbers.Number_Operations.pow(p, n) - 1; member_a++)
                {
                    for (int member_b = member_a; member_b <= Numbers.Number_Operations.pow(p, n) - 1; member_b++)
                    {
                        if (current_fx == Numbers.Number_Operations.mul(member_a, member_b, p))
                        {
                            fx_found = false;
                            break;
                        }
                    }
                    if (fx_found == false)
                    {
                        break;
                    }
                }

                if (fx_found)
                {
                    out_fx = current_fx;
                    break;
                }
            }

            return out_fx;
        }
        public int find_reversed_member()
        {
            if (member == 0)
            {
                return 0;
            }
            int reversed_degree = Numbers.Number_Operations.pow(p, n) - 1 - find_member_degree();
            int out_member = Numbers.Number_Operations.fx_pow(forming_element, fx, reversed_degree, p, n);
            return out_member;
        }
        public bool check_if_forming(int member)
        {
            bool is_forming = true;
            int current_member_powered = member;
            int degree = 1;
            int max_degree = Numbers.Number_Operations.pow(p, n);

            while (current_member_powered != 1)
            {
                current_member_powered = Numbers.Number_Operations.mul(current_member_powered, member, p);
                current_member_powered = Numbers.Number_Operations.normalizer(current_member_powered, fx, n, p);
                degree++;
                if (degree > max_degree)
                {
                    throw new Exceptions.NoValidNumbersFound();
                }
            }
            if (degree != Numbers.Number_Operations.pow(p, n) - 1)
            {
                is_forming = false;
            }
            return is_forming;
        }
        private int find_forming_element()
        {
            int forming_element = 0;
            int desired_degree = Numbers.Number_Operations.pow(p, n) - 1;       // Максимальный порядок соответствует числу элементов
            for (int current_element = 1; current_element <= desired_degree; current_element++)
            {
                if (check_if_forming(current_element))
                {
                    forming_element = current_element;
                    break;
                }
            }
            return forming_element;
        }
        // Если на входе 0, то возвращает первый образущий элемент,
        // Если 1 - то второй и т.д.
        public int find_forming_element(int random)
        {
            int random_steps = 0;
            int forming_element = 0;
            int desired_degree = Numbers.Number_Operations.pow(p, n) - 1;       // Максимальный порядок соответствует числу элементов
            for (int current_element = 1; current_element <= desired_degree; current_element++)
            {
                if (check_if_forming(current_element))
                {
                    forming_element = current_element;
                    if (random_steps == random)
                    {
                        break;
                    }
                    random_steps++;
                }
            }
            return forming_element;
        }
        public int find_member_degree()
        {
            if (member == 0)
            {
                return 0;
            }
            int current_member = 1;
            int degree = 0;
            while (current_member != member)
            {
                current_member = Numbers.Number_Operations.mul(current_member, forming_element, p);
                current_member = Numbers.Number_Operations.normalizer(current_member, fx, n, p);
                degree++;
            }
            return degree;
        }
        public int find_member_degree(int suggested_forming_element)
        {
            if (!check_if_forming(suggested_forming_element))
            {
                throw new Exceptions.CurrentMemberIsNotForming();
            }
            int current_member = 1;
            int degree = 0;
            while (current_member != member)
            {
                current_member = Numbers.Number_Operations.mul(current_member, suggested_forming_element, p);
                current_member = Numbers.Number_Operations.normalizer(current_member, fx, n, p);
                degree++;
            }
            return degree;
        }
        public int find_member_order()
        {
            if (member == 0)
            {
                return 0;
            }
            int degree = find_member_degree(forming_element);
            int max_degree = Numbers.Number_Operations.pow(p, n) - 1;
            int GCD = Numbers.Number_Operations.greatest_common_divisor(degree, max_degree);

            return max_degree / GCD;
        }
        public static Galois_Field operator +(Galois_Field a, Galois_Field b)
        {
            if (a.get_p() != b.get_p()
                || a.get_n() != b.get_n()
                || a.get_fx() != b.get_fx())
            {
                throw new Exceptions.IncompatibleGaloisFieldMembers();
            }

            int p = a.get_p();
            int n = a.get_n();
            int fx = a.get_fx();
            int forming_element = a.get_forming_element();

            int member = Numbers.Number_Operations.plus(a.get_member(), b.get_member(), p);

            Galois_Field c = new Galois_Field(p, n, member, fx, forming_element);

            return c;
        }
        public static Galois_Field operator -(Galois_Field a, Galois_Field b)
        {
            if (a.get_p() != b.get_p()
                || a.get_n() != b.get_n()
                || a.get_fx() != b.get_fx())
            {
                throw new Exceptions.IncompatibleGaloisFieldMembers();
            }

            int p = a.get_p();
            int n = a.get_n();
            int fx = a.get_fx();
            int forming_element = a.get_forming_element();

            int member = Numbers.Number_Operations.minus(a.get_member(), b.get_member(), p);

            Galois_Field c = new Galois_Field(p, n, member, fx, forming_element);

            return c;
        }
        public static Galois_Field operator *(Galois_Field a, Galois_Field b)
        {
            if (a.get_p() != b.get_p()
                || a.get_n() != b.get_n()
                || a.get_fx() != b.get_fx())
            {
                throw new Exceptions.IncompatibleGaloisFieldMembers();
            }

            int member = Numbers.Number_Operations.mul(a.get_member(), b.get_member(), a.get_p());
            member = Numbers.Number_Operations.normalizer(member, a.get_fx(), a.get_n(), a.get_p());

            Galois_Field c = new Galois_Field(a.get_p(), a.get_n(), member, a.get_fx(), a.get_forming_element());

            return c;
        }
        public static Galois_Field operator /(Galois_Field a, Galois_Field b)
        {
            if (a.get_p() != b.get_p()
                || a.get_n() != b.get_n()
                || a.get_fx() != b.get_fx())
            {
                throw new Exceptions.IncompatibleGaloisFieldMembers();
            }

            int member = Numbers.Number_Operations.mul(a.get_member(), b.find_reversed_member(), a.get_p());
            member = Numbers.Number_Operations.normalizer(member, a.get_fx(), a.get_n(), a.get_p());

            Galois_Field c = new Galois_Field(a.get_p(), a.get_n(), member, a.get_fx(), a.get_forming_element());

            return c;
        }

        private string visualizer(int current, int base_number)
        {
            string out_text = "";
            int max_power = Numbers.Number_Operations.get_power(current, base_number);

            for (int i = max_power; i > 0; i--)
            {
                int digit = Numbers.Number_Operations.get_coef_on_place(current, p, i);
                if (digit == 0)
                {
                    continue;
                }
                else if (i != max_power)
                {
                    out_text += " + ";
                }
                if (i != 1)
                {
                    if (digit == 1)
                    {
                        out_text += string.Format("X^{0}", i);
                    }
                    else
                    {
                        out_text += string.Format("{0}*X^{1}", digit, i);
                    }
                }
                else
                {
                    if (digit == 1)
                    {
                        out_text += "X";
                    }
                    else
                    {
                        out_text += string.Format("{0}*X", digit, i);
                    }
                }
            }
            int last_digit = Numbers.Number_Operations.get_coef_on_place(current, p, 0);
            if (last_digit != 0)
            {
                if (max_power == 0)
                {
                    out_text += string.Format("{0}", last_digit);
                }
                else
                {
                    out_text += string.Format(" + {0}", last_digit);
                }
            }

            return out_text;
        }
        public string visualize_member()
        {
            return visualizer(member, p);
        }
        public string visualize_irreducible_fx()
        {
            return visualizer(fx, p);
        }
        public string visualize_forming_element()
        {
            return visualizer(forming_element, p);
        }
    }
}

namespace Numbers
{
    public class Number_Operations
    {
        public static int greatest_common_divisor(int a, int b)
        {
            if (a == b)
            {
                return a;
            }
            if (a < b)
            {
                int c = a;
                a = b;
                b = c;
            }

            int r;
            do
            {
                r = a % b;
                a = b;
                b = r;
            }
            while(r != 0);
            return a;
        }
        public static int get_power(int number, int base_number)
        {
            int power = 0;
            int current_powered_number = 1;
            do
            {
                current_powered_number *= base_number;
                power++;
            }
            while (number >= current_powered_number);

            power--;

            return power;
        }
        public static int pow(int number, int degree)
        {
            if (degree == 0)
            {
                return 1;
            }
            else if (degree > 0)
            {
                int current_number = 1;
                for (int i = 0; i < degree; i++)
                {
                    current_number *= number;
                }
                return current_number;
            }
            else
            {
                return 0;
            }
        }
        public static int fx_pow(int member, int fx, int degree, int base_number, int n_limit)
        {
            if (degree == 0)
            {
                return 1;
            }
            else if (degree == 1)
            {
                return member;
            }
            else
            {
                int new_member = 1;
                for (int i = 0; i < degree; i++)
                {
                    new_member = mul(new_member, member, base_number);
                    new_member = normalizer(new_member, fx, n_limit, base_number);
                }

                return new_member;
            }
        }
        // Возвращает позицию от 0 до max_pow, а дальше нули.
        public static int get_coef_on_place(int number, int base_number, int position)
        {
            int current_coef = 0;
            for (int i = 0; i <= position; i++)
            {
                current_coef = number % base_number;
                number /= base_number;
            }
            return current_coef;
        }
        public static int set_coef_on_place(int number, int base_number, int position, int replace_coef)
        {
            int number_copy = number;
            int current_coef = 0;
            for (int i = 0; i <= position; i++)
            {
                current_coef = number % base_number;
                number /= base_number;
            }
            number_copy = number_copy
                - (current_coef % base_number) * pow(base_number, position)
                + (replace_coef % base_number) * pow(base_number, position);

            return number_copy;
        }
        public static int plus(int a, int b, int base_number)
        {
            int out_number = 0;
            int remainder_a = 0;
            int remainder_b = 0;
            int degree = 0;
            while (a != 0 || b != 0)
            {
                remainder_a = a % base_number;
                remainder_b = b % base_number;

                a /= base_number;
                b /= base_number;

                int int_plus = (remainder_a + remainder_b) % base_number;
                out_number += int_plus * pow(base_number, degree);

                degree++;
            }
            return out_number;
        }
        public static int minus(int a, int b, int base_number)
        {
            int out_number = 0;
            int remainder_a = 0;
            int remainder_b = 0;
            int degree = 0;
            while (a != 0 || b != 0)
            {
                remainder_a = a % base_number;
                remainder_b = b % base_number;

                a /= base_number;
                b /= base_number;

                int int_plus = (base_number + remainder_a - remainder_b) % base_number;
                out_number += int_plus * pow(base_number, degree);

                degree++;
            }
            return out_number;
        }
        public static int mul(int a, int b, int base_number)
        {
            int a_size = get_power(a, base_number) + 1;
            int b_size = get_power(b, base_number) + 1;

            int[] a_digits = new int[a_size];
            int i = 0;
            while (a > 0)
            {
                a_digits[i] = a % base_number;
                a /= base_number;
                i++;
            }

            int[] b_digits = new int[b_size];
            int j = 0;
            while (b > 0)
            {
                b_digits[j] = b % base_number;
                b /= base_number;
                j++;
            }

            int[] resulted_digits = new int[a_size + b_size - 1];
            for (i = 0; i < a_size + b_size - 1; i++)
            {
                for (j = 0; j <= i; j++)
                {
                    if (j < a_size && i - j < b_size)
                    {
                        int addable = a_digits[j] * b_digits[i - j] % base_number;
                        resulted_digits[i] = Numbers.Number_Operations.plus(resulted_digits[i],
                            addable,
                            base_number);
                    }
                }
            }

            int out_number = 0;
            int degree = 0;
            foreach (int digit in resulted_digits)
            {
                out_number += digit * pow(base_number, degree);
                degree++;
            }

            return out_number;
        }
        // Это деление на многочлен fx
        //
        // member - член поля Галуа
        // fx - неприводимый многочлен (Например x^4 + x + 1, где n_limit = 4)
        // n_limit - ограничение степени
        // base_number - основание p
        public static int normalizer(int member, int fx, int n_limit, int base_number)
        {
            int current_power = get_power(member, base_number);
            if (current_power < n_limit)
            {
                return member;
            }
            else
            {
                for (int i = current_power; i >= n_limit; i--)
                {
                    int digit = get_coef_on_place(member, base_number, i);
                    int multiplicated_digit = mul(digit, fx, base_number);
                    multiplicated_digit = mul(multiplicated_digit,
                        pow(base_number, i - n_limit),
                        base_number);

                    member = minus(member,
                        multiplicated_digit,
                        base_number);
                }
                return member;
            }
        }
        public static int[] polynomial_maker(int number, int base_number)
        {
            int degree = Numbers.Number_Operations.get_power(number, base_number);
            int[] digits = new int[degree + 1];

            int i = 0;
            while (number != 0)
            {
                digits[i++] = number % base_number;
                number /= base_number;
            }

            return digits;
        }
    }
}

namespace Dictionaries
{
    public class Dictionaries
    {
        public static char[] dict_base2_degree5 = [' ', ',', '.', '!', '?', '-', 'A', 'B', 'C', 'D',
                                                   'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N',
                                                   'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X',
                                                   'Y', 'Z'];

        public static char[] dict_base2_degree6 = [' ', ',', '.', '!', '?', '-', '(', ')', ';', ':',
                                                   '_', '*', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H',
                                                   'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R',
                                                   'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b',
                                                   'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l',
                                                   'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v',
                                                   'w', 'x', 'y', 'z'];

        public static char[] dict_base2_degree7 = [' ', ',', '.', '!', '?', '-', '(', ')', ';', ':',
                                                   'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J',
                                                   'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T',
                                                   'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd',
                                                   'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n',
                                                   'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x',
                                                   'y', 'z', 'А', 'Б', 'В', 'Г', 'Д', 'Е', 'Ё', 'Ж',
                                                   'З', 'И', 'Й', 'К', 'Л', 'М', 'Н', 'О', 'П', 'Р',
                                                   'С', 'Т', 'У', 'Ф', 'Х', 'Ц', 'Ч', 'Ш', 'Щ', 'Ъ',
                                                   'Ы', 'Ь', 'Э', 'Ю', 'Я', 'а', 'б', 'в', 'г', 'д',
                                                   'е', 'ё', 'ж', 'з', 'и', 'й', 'к', 'л', 'м', 'н',
                                                   'о', 'п', 'р', 'с', 'т', 'у', 'ф', 'х', 'ц', 'ч',
                                                   'ш', 'щ', 'ъ', 'ы', 'ь', 'э', 'ю', 'я'];
    }
}

namespace Ciphers
{
    public class AffineCipher
    {
        private static int find_position_in_dict(char symbol, char[] dict)
        {
            int i = 0;
            foreach(char c in dict)
            {
                if (c == symbol)
                {
                    return i;
                }
                else
                {
                    i++;
                }
            }
            return -1;
        }
        public static string encrypt_default(string input, Galois_Field.Galois_Field key_a, Galois_Field.Galois_Field key_b)
        {
            if (key_a.get_p() != key_b.get_p()
                || key_a.get_n() != key_b.get_n()
                || key_a.get_fx() != key_b.get_fx())
            {
                throw new Exceptions.IncompatibleGaloisFieldMembers();
            }

            string output = "";
            int p = key_a.get_p();
            int n = key_b.get_n();
            int fx = key_a.get_fx();
            int max_number = Numbers.Number_Operations.pow(p, n);

            foreach(char c in input)
            {
                int number = (int)c;
                if (number >= max_number)
                {
                    throw new Exceptions.IncompatibleCharAndField();
                }
                Galois_Field.Galois_Field member = new Galois_Field.Galois_Field(p, n, number, fx);
                member = member * key_a + key_b;
                char member_char = (char)member.get_member();
                output += member_char;
            }
            return output;
        }
        public static string decrypt_default(string input, Galois_Field.Galois_Field key_a, Galois_Field.Galois_Field key_b)
        {
            if (key_a.get_p() != key_b.get_p()
                || key_a.get_n() != key_b.get_n()
                || key_a.get_fx() != key_b.get_fx())
            {
                throw new Exceptions.IncompatibleGaloisFieldMembers();
            }

            string output = "";
            int p = key_a.get_p();
            int n = key_b.get_n();
            int fx = key_a.get_fx();
            int max_number = Numbers.Number_Operations.pow(p, n);

            foreach (char c in input)
            {
                int number = (int)c;
                if (number >= max_number)
                {
                    throw new Exceptions.IncompatibleCharAndField();
                }
                Galois_Field.Galois_Field member = new Galois_Field.Galois_Field(p, n, number, fx);
                member = (member - key_b) / key_a;
                char member_char = (char)member.get_member();
                output += member_char;
            }
            return output;
        }
        public static string encrypt_with_dict(string input, Galois_Field.Galois_Field key_a, Galois_Field.Galois_Field key_b, char[] dict)
        {
            if (key_a.get_p() != key_b.get_p()
                || key_a.get_n() != key_b.get_n()
                || key_a.get_fx() != key_b.get_fx())
            {
                throw new Exceptions.IncompatibleGaloisFieldMembers();
            }

            string output = "";
            int p = key_a.get_p();
            int n = key_b.get_n();
            int fx = key_a.get_fx();

            int member_count = Numbers.Number_Operations.pow(p, n);
            if (member_count != dict.Length)
            {
                throw new Exceptions.WrongDictForField();
            }

            foreach (char c in input)
            {
                int number = find_position_in_dict(c, dict);
                if (number == -1)
                {
                    throw new Exceptions.NonexistingCharInDict();
                }

                Galois_Field.Galois_Field member = new Galois_Field.Galois_Field(p, n, number, fx);
                member = member * key_a + key_b;

                output += dict[member.get_member()];
            }
            return output;
        }
        public static string decrypt_with_dict(string input, Galois_Field.Galois_Field key_a, Galois_Field.Galois_Field key_b, char[] dict)
        {
            if (key_a.get_p() != key_b.get_p()
                || key_a.get_n() != key_b.get_n()
                || key_a.get_fx() != key_b.get_fx())
            {
                throw new Exceptions.IncompatibleGaloisFieldMembers();
            }

            string output = "";
            int p = key_a.get_p();
            int n = key_b.get_n();
            int fx = key_a.get_fx();

            int member_count = Numbers.Number_Operations.pow(p, n);
            if (member_count != dict.Length)
            {
                throw new Exceptions.WrongDictForField();
            }

            
            foreach (char c in input)
            {
                int number = find_position_in_dict(c, dict);
                if (number == -1)
                {
                    throw new Exceptions.NonexistingCharInDict();
                }

                Galois_Field.Galois_Field member = new Galois_Field.Galois_Field(p, n, number, fx);
                member = (member - key_b) / key_a;

                output += dict[member.get_member()];
            }
            return output;
        }
    }
}

namespace ConsoleProgram
{
    public class ConsoleProgram
    {
        public static Galois_Field.Galois_Field get_galois_field_member()
        {
            Galois_Field.Galois_Field Element;

            while (true)
            {
                try
                {
                    Console.WriteLine("Выбран инструмент для работы с полем Галуа!");
                    Console.WriteLine("Введите основные параметры поля Галуа:\n");

                    Console.Write("Введите основание p: ");
                    int p = Int32.Parse(Console.ReadLine());
                    if (p < 2)
                    {
                        throw new Exceptions.WrongInput();
                    }

                    Console.Write("Введите степень n: ");
                    int n = Int32.Parse(Console.ReadLine());
                    if (n < 2)
                    {
                        throw new Exceptions.WrongInput();
                    }

                    Console.Write("Подставьте в многочлен поля X={0} и впишите результат: ", p);
                    int member = Int32.Parse(Console.ReadLine());
                    if (member < 0)
                    {
                        throw new Exceptions.WrongInput();
                    }

                    Console.Write("Аналогично впишите число для неприводимого многочлена\nЕсли нужно выбрать автоматически, то впишите 0: ");
                    int fx = Int32.Parse(Console.ReadLine());
                    if (fx < Numbers.Number_Operations.pow(p, n) && fx != 0)
                    {
                        throw new Exceptions.WrongInput();
                    }

                    Element = new Galois_Field.Galois_Field(p, n, member);

                    if (fx != 0)
                    {
                        if (Element.check_if_fx_irreducible(fx))
                        {
                            Element = new Galois_Field.Galois_Field(p, n, member, fx);
                        }
                        else
                        {
                            throw new Exceptions.WrongIrreduciblePolynomial();
                        }
                    }

                    Console.Write("Точно так же введите образующий элемент\nЕсли без него, то 0: ");
                    int forming_element = Int32.Parse(Console.ReadLine());
                    if (forming_element < 1 && forming_element != 0)
                    {
                        throw new Exceptions.WrongInput();
                    }

                    if (forming_element != 0)
                    {
                        if (Element.check_if_forming(forming_element))
                        {
                            Element = new Galois_Field.Galois_Field(p, n, member, Element.get_fx(), forming_element);
                        }
                        else
                        {
                            throw new Exceptions.CurrentMemberIsNotForming();
                        }
                    }

                }
                catch (WrongIrreduciblePolynomial)
                {
                    show_wrong_irreducible_polynomial_error();

                    continue;
                }
                catch (CurrentMemberIsNotForming)
                {
                    show_current_member_is_not_forming_error();

                    continue;
                }
                catch
                {
                    show_wrong_input_error();

                    continue;
                }

                break;
            }

            return Element;
        }
        public static Galois_Field.Galois_Field form_element_based_on_existing(Galois_Field.Galois_Field existing)
        {
            Console.WriteLine("Создание элемента поля Галуа на основе имеющегося.\n");
            Console.Write("Введите элемент, представленный в виде числа: ");
            int member = Int32.Parse(Console.ReadLine());
            Console.WriteLine("\n");


            return new Galois_Field.Galois_Field(existing.get_p(),
                existing.get_n(),
                member,
                existing.get_fx(),
                existing.get_forming_element());
        }
        public static void show_wrong_input_error()
        {
            Console.Clear();

            MessageBox.Show(text: "Введено неправильное значение!",
                            caption: "Ошибка",
                            buttons: MessageBoxButtons.OK,
                            icon: MessageBoxIcon.Error);
        }
        public static void show_wrong_irreducible_polynomial_error()
        {
            Console.Clear();

            MessageBox.Show(text: "Введён приводимый многочлен!",
                            caption: "Ошибка",
                            buttons: MessageBoxButtons.OK,
                            icon: MessageBoxIcon.Error);
        }
        public static void show_current_member_is_not_forming_error()
        {
            Console.Clear();

            MessageBox.Show(text: "Введён не образующий элемент!",
                            caption: "Ошибка",
                            buttons: MessageBoxButtons.OK,
                            icon: MessageBoxIcon.Error);
        }
        public ConsoleProgram()
        {
            int chosen_program;
            while (true)
            {
                try
                {
                    Console.WriteLine("Выберите режим работы программы:");
                    Console.WriteLine("  1) Инструмент для работы с полем Галуа;");
                    Console.WriteLine("  2) Инструмент для шифрования;");
                    Console.WriteLine("  3) Прекратить.\n");
                    Console.Write("Выбранный режим: ");

                    chosen_program = Int32.Parse(Console.ReadLine());
                    if (chosen_program != 1 && chosen_program != 2 && chosen_program != 3)
                    {
                        throw new Exceptions.WrongInput();
                    }
                    Console.Clear();
                    break;
                }
                catch
                {
                    show_wrong_input_error();

                    continue;
                }
            }

            if (chosen_program == 1)
            {
                GaloisFieldTool tool = new GaloisFieldTool();
            }
            if (chosen_program == 2)
            {
                CipherTool tool = new CipherTool();
            }
        }
        public class GaloisFieldTool
        {
            public GaloisFieldTool()
            {
                Galois_Field.Galois_Field member_a = get_galois_field_member();
                Galois_Field.Galois_Field member_b = null;

                Console.Clear();
                Console.WriteLine("Отлично! Вы добавили элемент в поле Галуа!");

                int chosen_program;

                while (true)
                {
                    while (true)
                    {
                        try
                        {
                            Console.WriteLine("\nВизуализация элемента A: " + member_a.visualize_member());
                            Console.WriteLine("В виде числа: " + member_a.get_member() + "\n");
                            if (member_b != null)
                            {
                                Console.WriteLine("Визуализация элемента B: " + member_b.visualize_member());
                                Console.WriteLine("В виде числа: " + member_b.get_member() + "\n");
                            }
                            Console.WriteLine("Выберите дальнейшие действия:");
                            Console.WriteLine("  1) Вывести образующий элемент;");
                            Console.WriteLine("  2) Вывести неприводимый многочлен;");
                            Console.WriteLine("  3) Вычислить степень элемента;");
                            Console.WriteLine("  4) Вычислить порядок элемента;");
                            Console.WriteLine("  5) Найти обратный элемент;");
                            Console.WriteLine("  6) Возвести в степень;");
                            Console.WriteLine("  7) Изменить A;");
                            Console.WriteLine("  8) Изменить B;");
                            Console.WriteLine("  9)  + B;");
                            Console.WriteLine("  10) - B;");
                            Console.WriteLine("  11) * B;");
                            Console.WriteLine("  12) / B;");
                            Console.WriteLine("  13) Выйти в главное меню.\n");
                            Console.Write("Выбранный режим: ");

                            chosen_program = Int32.Parse(Console.ReadLine());
                            if (chosen_program < 1 || chosen_program > 13)
                            {
                                throw new Exceptions.WrongInput();
                            }
                            Console.Clear();
                            break;

                        }
                        catch
                        {
                            show_wrong_input_error();

                            Console.Clear();
                            continue;
                        }
                    }
                    bool want_to_restart = false;
                    switch (chosen_program)
                    {
                        case 1:
                            Console.WriteLine("Образующий элемент: " + member_a.visualize_forming_element());
                            Console.WriteLine("В виде числа: " + member_a.get_forming_element());
                            break;
                        case 2:
                            Console.WriteLine("Неприводимый многочлен: " + member_a.visualize_irreducible_fx());
                            Console.WriteLine("В виде числа: " + member_a.get_fx());
                            break;
                        case 3:
                            Console.WriteLine("Степень данного элемента: " + member_a.find_member_degree());
                            break;
                        case 4:
                            Console.WriteLine("Порядок данного элемента: {0}\nМаксимальный порядок: {1}",
                                member_a.find_member_order(),
                                Numbers.Number_Operations.pow(member_a.get_p(), member_a.get_n()) - 1);
                            break;
                        case 5:
                            Galois_Field.Galois_Field reversed = new Galois_Field.Galois_Field(member_a.get_p(),
                                member_a.get_n(),
                                member_a.find_reversed_member(),
                                member_a.get_fx(),
                                member_a.get_forming_element());

                            Console.WriteLine("Обратный элемент: " + reversed.visualize_member());
                            Console.WriteLine("В виде числа: " + reversed.get_member());
                            break;
                        case 6:
                            while (true)
                            {
                                try
                                {
                                    Console.WriteLine("Элемент A: " + member_a.visualize_member() + "\n");
                                    Console.Write("Введите степень: ");
                                    int degree = Int32.Parse(Console.ReadLine());
                                    degree = degree % (Numbers.Number_Operations.pow(member_a.get_p(), member_a.get_n()) - 1);

                                    if (degree == 0)
                                    {
                                        Console.WriteLine("\nИтог: 1");
                                    }
                                    if (degree > 0)
                                    {
                                        Galois_Field.Galois_Field out_member = new Galois_Field.Galois_Field(member_a.get_p(),
                                            member_a.get_n(),
                                            1,
                                            member_a.get_fx(),
                                            member_a.get_forming_element());

                                        for (int i = 0; i < degree; i++)
                                        {
                                            out_member *= member_a;
                                        }

                                        Console.WriteLine("\nИтог: " + out_member.visualize_member());
                                        Console.WriteLine("В виде числа: " + out_member.get_member());
                                    }
                                    if (degree < 0)
                                    {
                                        Galois_Field.Galois_Field out_member = new Galois_Field.Galois_Field(member_a.get_p(),
                                            member_a.get_n(),
                                            1,
                                            member_a.get_fx(),
                                            member_a.get_forming_element());

                                        for (int i = 0; i > degree; i--)
                                        {
                                            out_member /= member_a;
                                        }

                                        Console.WriteLine("\nИтог: " + out_member.visualize_member());
                                        Console.WriteLine("В виде числа: " + out_member.get_member());
                                    }

                                    break;
                                }
                                catch
                                {
                                    show_wrong_input_error();

                                    Console.Clear();
                                }
                            }
                            break;
                        case 7:
                            member_a = form_element_based_on_existing(member_a);
                            Console.WriteLine("Новый элемент A: " + member_a.visualize_member());
                            Console.WriteLine("В виде числа: " + member_a.get_member());
                            break;
                        case 8:
                            member_b = form_element_based_on_existing(member_a);
                            Console.WriteLine("Новый элемент B: " + member_b.visualize_member());
                            Console.WriteLine("В виде числа: " + member_b.get_member());
                            break;
                        case 9:
                            if (member_b == null) { member_b = form_element_based_on_existing(member_a); }
                            Galois_Field.Galois_Field resulted9 = member_a + member_b;
                            Console.WriteLine("Сумма A + B: " + resulted9.visualize_member());
                            Console.WriteLine("В виде числа: " + resulted9.get_member());
                            break;
                        case 10:
                            if (member_b == null) { member_b = form_element_based_on_existing(member_a); }
                            Galois_Field.Galois_Field resulted10 = member_a - member_b;
                            Console.WriteLine("Разность A - B: " + resulted10.visualize_member());
                            Console.WriteLine("В виде числа: " + resulted10.get_member());
                            break;
                        case 11:
                            if (member_b == null) { member_b = form_element_based_on_existing(member_a); }
                            Galois_Field.Galois_Field resulted11 = member_a * member_b;
                            Console.WriteLine("Произведение A * B: " + resulted11.visualize_member());
                            Console.WriteLine("В виде числа: " + resulted11.get_member());
                            break;
                        case 12:
                            if (member_b == null) { member_b = form_element_based_on_existing(member_a); }
                            Galois_Field.Galois_Field resulted12 = member_a / member_b;
                            Console.WriteLine("Частное A / B: " + resulted12.visualize_member());
                            Console.WriteLine("В виде числа: " + resulted12.get_member());
                            break;
                        case 13:
                            want_to_restart = true;
                            break;
                    }
                    if (!want_to_restart)
                    {
                        Console.Write("\nПовторить? Нажмите Enter");
                        string stop_string = Console.ReadLine();
                        if (!stop_string.Equals(""))
                        {
                            break;
                        }
                        Console.Clear();
                    }
                    else
                    {
                        break;
                    }
                }
                Console.Clear();
                ConsoleProgram program = new ConsoleProgram();

            }
        }
        public class CipherTool
        {
            public CipherTool()
            {
                int chosen_dict;
                char[] dict = null;
                string output_chosen_dict = "Вы выбрали ";

                int n = 0;
                int p = 0;
                Galois_Field.Galois_Field member_a, member_b;
                int number_a, number_b;

                while (true)
                {
                    try
                    {
                        Console.Clear();
                        Console.WriteLine("Выберите словарь:");
                        Console.WriteLine("  1) Default;");
                        Console.WriteLine("  2) dict_base2_degree5;");
                        Console.WriteLine("  3) dict_base2_degree6;");
                        Console.WriteLine("  4) dict_base2_degree7;\n");
                        Console.Write("Выбранный режим: ");

                        chosen_dict = Int32.Parse(Console.ReadLine());
                        if (chosen_dict != 1 && chosen_dict != 2 && chosen_dict != 3 && chosen_dict != 4)
                        {
                            throw new Exceptions.WrongInput();
                        }
                        Console.Clear();

                        if (chosen_dict == 1)
                        {
                            Console.WriteLine("Введите необходимые параметры поля:");
                            Console.Write("  p = ");
                            p = Int32.Parse(Console.ReadLine());
                            Console.Write("  n = ");
                            n = Int32.Parse(Console.ReadLine());
                            output_chosen_dict += "Default";
                        }
                        if (chosen_dict == 2)
                        {
                            p = 2;
                            n = 5;
                            output_chosen_dict += "dict_base2_degree5";
                            dict = Dictionaries.Dictionaries.dict_base2_degree5;
                        }
                        if (chosen_dict == 3)
                        {
                            p = 2;
                            n = 6;
                            output_chosen_dict += "dict_base2_degree6";
                            dict = Dictionaries.Dictionaries.dict_base2_degree6;
                        }
                        if (chosen_dict == 4)
                        {
                            p = 2;
                            n = 7;
                            output_chosen_dict += "dict_base2_degree7";
                            dict = Dictionaries.Dictionaries.dict_base2_degree7;
                        }

                        Console.Write("Введите ключ A в виде числа (> 0):  ");
                        number_a = Int32.Parse(Console.ReadLine());
                        if (number_a < 1)
                        {
                            throw new Exceptions.WrongInput();
                        }

                        Console.Write("Введите ключ B в виде числа (>= 0): ");
                        number_b = Int32.Parse(Console.ReadLine());
                        if (number_b < 0)
                        {
                            throw new Exceptions.WrongInput();
                        }
                        Console.Clear();
                        break;
                    }
                    catch
                    {
                        ConsoleProgram.show_wrong_input_error();

                        continue;
                    }
                }

                int chosen_program;
                member_a = new Galois_Field.Galois_Field(p, n, number_a);
                member_b = new Galois_Field.Galois_Field(p, n, number_b);

                while (true)
                {
                    try
                    {
                        bool want_to_restart = false;
                        Console.WriteLine(output_chosen_dict);
                        Console.WriteLine("Выберите дальнейшие действия:");
                        Console.WriteLine("  1) Зашифровать;");
                        Console.WriteLine("  2) Расшифровать;");
                        Console.WriteLine("  3) Тест шифровальщика;");
                        Console.WriteLine("  4) Выйти в главное меню.\n");
                        Console.Write("Выбранный режим работы: ");

                        chosen_program = Int32.Parse(Console.ReadLine());
                        if (chosen_dict != 1 && chosen_dict != 2 && chosen_dict != 3 && chosen_dict != 4)
                        {
                            throw new Exceptions.WrongInput();
                        }
                        Console.Clear();

                        if (chosen_program == 4)
                        {
                            break;
                        }

                        Console.WriteLine("Введите текст:");
                        string input = Console.ReadLine();
                        string output = "";
                        Console.Clear();

                        Console.WriteLine("Используемый словарь: " + output_chosen_dict.Remove(0, 11));
                        if (chosen_program == 1)
                        {
                            Console.WriteLine("Введённый текст: \"" + input + "\"");
                            if (chosen_dict == 1)
                            {
                                output = Ciphers.AffineCipher.encrypt_default(input, member_a, member_b);
                            }
                            else
                            {
                                output = Ciphers.AffineCipher.encrypt_with_dict(input, member_a, member_b, dict);
                            }
                            Console.WriteLine("Шифр-текст: \"" + output + "\"");
                        }
                        if (chosen_program == 2)
                        {
                            Console.WriteLine("Введённый шифр-текст: \"" + input + "\"");
                            if (chosen_dict == 1)
                            {
                                output = Ciphers.AffineCipher.decrypt_default(input, member_a, member_b);
                            }
                            else
                            {
                                output = Ciphers.AffineCipher.decrypt_with_dict(input, member_a, member_b, dict);
                            }
                            Console.WriteLine("Исходный текст:       \"" + output + "\"");
                        }
                        if (chosen_program == 3)
                        {
                            string output2;
                            Console.WriteLine("\nТест работы программы:");
                            Console.WriteLine("Введённый текст: \"" + input + "\"");
                            if (chosen_dict == 1)
                            {
                                output = Ciphers.AffineCipher.encrypt_default(input, member_a, member_b);
                                Console.WriteLine("Шифр-текст:      \"" + output + "\"");
                                output2 = Ciphers.AffineCipher.decrypt_default(output, member_a, member_b);
                            }
                            else
                            {
                                output = Ciphers.AffineCipher.encrypt_with_dict(input, member_a, member_b, dict);
                                Console.WriteLine("Шифр-текст:      \"" + output + "\"");
                                output2 = Ciphers.AffineCipher.decrypt_with_dict(output, member_a, member_b, dict);
                            }
                            Console.WriteLine("Расшифровка:     \"" + output2 + "\"");

                            if (input.Equals(output2))
                            {
                                Console.WriteLine("\nПрограмма отработала верно!");
                            }
                            else
                            {
                                Console.WriteLine("\nЧто-то пошло не так...");
                            }
                        }

                        Console.Write("\nПовторить? Нажмите Enter");
                        string stop_string = Console.ReadLine();
                        if (!stop_string.Equals(""))
                        {
                            break;
                        }
                        Console.Clear();
                    }
                    catch
                    {
                        ConsoleProgram.show_wrong_input_error();
                        Console.Clear();
                        continue;
                    }
                }
                Console.Clear();
                ConsoleProgram program = new ConsoleProgram();
            }
        }
    }
}

namespace Program
{
    static class Program
    {
        static void Main()
        {
            Console.OutputEncoding = Encoding.Unicode;
            Console.InputEncoding = Encoding.Unicode;

            ConsoleProgram.ConsoleProgram program = new ConsoleProgram.ConsoleProgram();
        }
    }
}


/*=============================
  Для демонстрации инструмента

  Console.WriteLine(Numbers.Number_Operations.get_power(15, 2));
  Galois_Field.Galois_Field member1 = new Galois_Field.Galois_Field(2, 8, 17);
  Galois_Field.Galois_Field member2 = new Galois_Field.Galois_Field(2, 8, 180);

  Galois_Field.Galois_Field member3 = member1 * member2;

  Console.WriteLine("Неприводимый многочлен: " + member1.get_fx());
  Console.WriteLine("Обратный к member: " + member1.find_reversed_member());
  Console.WriteLine("Образующий элемент: " + member1.get_forming_element());
  Console.WriteLine("Степень этого элемента: " + member1.find_member_degree());

  Console.WriteLine("member1 * reversed_member1: " + member3.get_member());
  Console.WriteLine("member1 / member1: " + (member1 / member1).get_member());

  Galois_Field.Galois_Field member4 = new Galois_Field.Galois_Field(2, 8, 7, 283, 3);

  Console.WriteLine("Визуализация образующего: " + member4.visualize_forming_element());
  Console.WriteLine("Визуализация неприводимого многочлена: " + member4.visualize_irreducible_fx());
  Console.WriteLine("Визуализация элемента: " + member4.visualize_member());
*/

/*============================= 
  Для демонстрации шифра через непроизвольный словарь

  Galois_Field.Galois_Field key_a = new Galois_Field.Galois_Field(2, 8, 17);
  Galois_Field.Galois_Field key_b = new Galois_Field.Galois_Field(2, 8, 54);

  string text = "Hello, World!";

  text = Ciphers.AffineCipher.encrypt_default(text, key_a, key_b);
  Console.WriteLine(text);
  text = Ciphers.AffineCipher.decrypt_default(text, key_a, key_b);
  Console.WriteLine(text);
*/

/*=============================
  Для демонстрации шифра через произвольный словарь

  Galois_Field.Galois_Field key_a = new Galois_Field.Galois_Field(2, 6, 17);
  Galois_Field.Galois_Field key_b = new Galois_Field.Galois_Field(2, 6, 54);

  string text = "Hello, World!";

  text = Ciphers.AffineCipher.encrypt_with_dict(text, key_a, key_b, Dictionaries.Dictionaries.dict_base2_degree6);
  Console.WriteLine(text);
  text = Ciphers.AffineCipher.decrypt_with_dict(text, key_a, key_b, Dictionaries.Dictionaries.dict_base2_degree6);
  Console.WriteLine(text);
*/