namespace NthDimension.Math
{
    using System;

    public class SolverInt
    {
        public int[] CalcSolution(int[,] matrix)                                                                            // Определя се набора от минимални по стойност цели решения на системата
        {
            int zeroRowsCount;                                                                                              // Броят на нулевите редове
            do
            {
                matrix = RemoveZeroRows(matrix);                                                                            // Премахват се редовете, които са съставени само от нули
                matrix = GaussianElimination(matrix, out zeroRowsCount);                                                    // Премахват се коефициентите пред съответните неизвестни по метода на Гаус
            }
            while (zeroRowsCount > 0);                                                                                      // Действията се повтарят докато се получи обработена матрица по метода на Гаус без линейнозависими редове

            int rowsCount = matrix.GetLength(0);                                                                            // Броя на редовете от матрицата
            int colsCount = matrix.GetLength(1);                                                                            // Броя на колоните

            if (colsCount - rowsCount == 1)                                                                                 // Реакцията може да бъде изравнена само когато броят на веществата е с едно по-голям от броя на химичните елементи
            {
                int[] solutions = new int[colsCount];                                                                       // Масивът, където ще се запише намереният набор от решения
                solutions[colsCount - 1] = Math.Abs(matrix[rowsCount - 1, colsCount - 2]);                                  // Последното решение винаги е равно на стойността в последния ред и предпоследната колона от матрицата

                for (int row = rowsCount - 1; row >= 0; row--)                                                              // От последният ред нагоре последователно се намират решенията на матрицата
                {
                    int num = 0;                                                                                            // Променливата, в която ще се изчиси числителят
                    for (int col = row + 1; col < colsCount; col++)                                                         // Обхождат се колоните като се започва от ненулев елемент
                    {
                        num -= matrix[row, col] * solutions[col];                                                           // Събират (със знак минус отпред) се произведенията на елементите от определената позиция до края на реда с намерените решениия до момента
                    }
                    int denom = matrix[row, row];                                                                           // Знаменателят е достигнатата стойност от матрицата

                    int gcd = GreatestCommonDivisor(num, denom);                                                            // Намира се НОД
                    num /= gcd;                                                                                             // Числителят се съкращава на него
                    denom /= gcd;                                                                                           // и знаменателят

                    if (Math.Abs(num) % Math.Abs(denom) != 0)                                                               // Ако числителят не се дели на знаменателя
                    {
                        solutions[row] = Math.Abs(num);                                                                     // За решение се взема числителят,
                        for (int pos = colsCount - 1; pos > row; pos--) solutions[pos] *= Math.Abs(denom);                  // а всички намерени до момента решения се умножават по знаменателя. За следващите решения предходните ще участват с новите си стойности в числителя и така няма нужда следващите решения да се умножават по знаменателя.
                    }
                    else solutions[row] = num / denom;                                                                      // Ако числителят се дели на знаменателя решението е частното на двете
                }

                return solutions;                                                                                           // Връщат се намерения набор от решения
            }

            return null;                                                                                                    // Ако системата не може да бъде решена не се връща нищо
        }

        private int[,] GaussianElimination(int[,] matrix, out int zeroRowsCount)                                            // Обработва матрицата по метода на Гаус
        {
            int rowsCount = matrix.GetLength(0);
            int colsCount = matrix.GetLength(1);
            zeroRowsCount = 0;

            for (int upperRow = 0; upperRow < rowsCount - 1; upperRow++)                                                // Редът, с който ще се зануляват стойностите пред поредното неизвестно
            {
                if (zeroRowsCount == 0)                                                                                 // Елиминирането се извършва само за системи с ненулеви редове
                {
                    DivideRow(matrix, upperRow);                                                                        // За опростяване на пресмятанията, стойностите в реда се разделят на техния НОД
                    int upperNum = matrix[upperRow, upperRow];                                                          // Взема се стойността от реда, с която ще се занулява

                    for (int lowerRow = upperRow + 1; lowerRow < rowsCount; lowerRow++)                                 // Долните редове - чиито съответни стойности ще се зануляват
                    {
                        if (zeroRowsCount == 0)
                        {
                            int lowerNum = matrix[lowerRow, upperRow];                                                  // Текущата стойност, която ще бъде занулена
                            if (lowerNum != 0)                                                                          // Ако стойността си е 0 - няма какво да се прави
                            {
                                if (upperNum == 0 || Math.Abs(lowerNum) < Math.Abs(upperNum))                           // Ако горната стойност е 0 или долната стойност е по-малка по абсолютна стойност, не може да се унищожи чрез целочислено умножение
                                {
                                    SwapRows(matrix, upperRow, lowerRow);                                               // затова двата реда си разменят местата
                                    upperNum = matrix[upperRow, upperRow];                                              // като се актуализира стойността на променливата, с която ще се занулява
                                    lowerRow--;                                                                         // понеже не се извършва друго (самото зануляване ще се извърши на следващото завъртане на цикъла) се връща стойността на итератора 
                                }
                                else                                                                                    // Ако долната стойност (която ще бъде унищожена е по-голяма по абсолютна стойност)
                                {                                                                                       // тя може да бъде занулена чрез подходящо умножение на двете стойности и след това събиране
                                    int lcm = LowestCommonMultiple(upperNum, lowerNum);                                 // За да се определят подходящите множители се изчислява НОК от двете стойности
                                    int upperMul = lcm / Math.Abs(upperNum);                                            // и се умножават по НОК разделен на съответната стойност (взета като положително число)
                                    int lowerMul = lcm / Math.Abs(lowerNum);                                            // понеже и самият НОК се изчислява винаги като положителнно число
                                    if (upperNum * lowerNum > 0) upperMul = -upperMul;                                  // След обработката двете стойности са едни и същи, но трябва да са с различни знаци за да се унищожат чрез събиране

                                    for (int col = upperRow; col < colsCount; col++)                                    // Обхожда се целият долен ред
                                    {
                                        int numToAdd = upperMul * matrix[upperRow, col];                                // Изчислява се стойността за всяка колона от горния ред
                                        matrix[lowerRow, col] *= lowerMul;                                              // Долният ред пък се умножава по съответния коефициент за него
                                        matrix[lowerRow, col] += numToAdd;                                              // и се извършва събирането, чрез което ще се занули стойността пред поредното неизвестно
                                    }

                                    FindZeroRows(matrix, out zeroRowsCount);                                            // Определя се броя на нулевите редове
                                    if (zeroRowsCount == 0) DivideRow(matrix, lowerRow);
                                }
                            }
                            else DivideRow(matrix, lowerRow);                                                           // При 0 пред съответното неизестно не се извършват други изчисления, освен ако е нужно, стойностите от реда да се разделят на НОД
                        }
                    }
                }
            }

            return matrix;                                                                                              // Връща се обработената матрица
        }

        private bool[] FindZeroRows(int[,] matrix, out int zeroRowsCount)                                               // Търси позициите и броя на нулевите редове
        {
            int rowsCount = matrix.GetLength(0);
            int colsCount = matrix.GetLength(1);

            zeroRowsCount = 0;                                                                                          // Броят на редовете изцяло съставени от нули
            bool[] zeroRows = new bool[rowsCount];                                                                      // Масивът, в който ще се запомнят позициите на нулевите редове
            for (int row = 0; row < rowsCount; row++)
            {
                bool rowIsZeroRow = true;                                                                               // Първоначално се приема, че поредният ред е нулев
                for (int col = 0; col < colsCount; col++)
                {
                    if (matrix[row, col] != 0) rowIsZeroRow = false;                                                    // Ако се срещне стойност различна от нула - редът не е нулев
                }

                if (rowIsZeroRow) zeroRowsCount++;                                                                      // Ако редът се е оказал нулев се преброява
                zeroRows[row] = rowIsZeroRow;                                                                           // В масива на съответният ред се запомня какъв е реда
            }

            return zeroRows;                                                                                            // Връща се масива със съхранените видове редове 
        }

        private int[,] RemoveZeroRows(int[,] matrix)                                                                    // Премахва нулевите редове
        {
            int rowsCount = matrix.GetLength(0);
            int colsCount = matrix.GetLength(1);

            int zeroRowsCount;                                                                                          // Броят на нулевите редове
            bool[] zeroRows = FindZeroRows(matrix, out zeroRowsCount);                                                  // Масивът със стойностите за съответните редове

            int currentRow = 0;                                                                                         // Позицията на поредния ред в новият масив (без нулевите редове)
            int[,] noZeroRowsMatrix = new int[rowsCount - zeroRowsCount, colsCount];
            for (int row = 0; row < rowsCount; row++)
            {
                if (!zeroRows[row])                                                                                     // Прехвърлят се само ненулевите редове
                {
                    for (int col = 0; col < colsCount; col++)
                    {
                        noZeroRowsMatrix[currentRow, col] = matrix[row, col];
                    }
                    currentRow++;                                                                                       // Преминава се към следващия ред от матрицата с ненулеви редове
                }
            }

            return noZeroRowsMatrix;                                                                                    // Връща се получената матрица (с ненулеви редове)
        }

        private void DivideRow(int[,] matrix, int row)                                                                  // Разделя реда на НОД от стойностите в него
        {
            int colsCount = matrix.GetLength(1);

            int gcd = GreatestCommonDivisorInRow(matrix, row);                                                          // Определя НОД
            if (gcd > 1)                                                                                                // Дели се само, ако има смисъл
            {
                for (int col = 0; col < colsCount; col++) matrix[row, col] /= gcd;
            }
        }

        private void SwapRows(int[,] matrix, int upperRow, int lowerRow)                                                // По зададена матрица и два реда разменя местата им
        {
            int colsCount = matrix.GetLength(1);
            for (int col = 0; col < colsCount; col++)
            {
                int tmp = matrix[lowerRow, col];
                matrix[lowerRow, col] = matrix[upperRow, col];
                matrix[upperRow, col] = tmp;
            }
        }

        private int GreatestCommonDivisor(int a, int b)                                                                 // Определя НОД на две естествени числа по метода на Евклид с изваждане
        {
            if (a < 0) a = -a;                                                                                          // Методът работи само с естествени числа
            if (b < 0) b = -b;
            while (a != b)
            {
                if (a > b) a -= b;
                else b -= a;
            }
            return a;
        }

        private int GreatestCommonDivisorInRow(int[,] matrix, int row)                                                  // Определя НОД от стойностите на цял ред от матрица
        {
            int rowsCount = matrix.GetLength(0);
            int colsCount = matrix.GetLength(1);
            int[,] matrixCopy = new int[rowsCount, colsCount];                                                          // Създава се копие на матрицата за работа върху него за да не се правят промени в оригиналната матрица
            Array.Copy(matrix, matrixCopy, rowsCount * colsCount);                                                      // Копират се всички елементи от старата матрица

            for (int pos = 0; pos < colsCount; pos++)
            {
                if (matrixCopy[row, pos] < 0) matrixCopy[row, pos] = -matrixCopy[row, pos];                             // За определяне на НОД всички елементи от реда трябва да са естествени числа 
            }

            int gcd = MaxNumberInRow(matrixCopy, row);                                                                  // Приема се, че НОД е най-голямото число от реда
            bool gcdFound;                                                                                              // Променлива, която показва дали НОД е намерен
            do
            {
                gcdFound = true;                                                                                        // Първоначално се приема, че НОД е намерен
                for (int pos = 0; pos < colsCount; pos++)                                                               // Проверява се дали предполагаемият НОД дели всички стойности
                {
                    if (matrixCopy[row, pos] % gcd != 0) gcdFound = false;                                              // Ако не
                }
                gcd--;                                                                                                  // се проверява следващото по-малко с 1-ца число
            }
            while (!gcdFound);

            return gcd + 1;                                                                                             // Понеже намаляването е станало след проверката се връща предната стойност
        }

        private int LowestCommonMultiple(int a, int b)                                                                  // НОК на две числа
        {
            if (a < 0) a = -a;                                                                                          // Методът работи само с естествени числа
            if (b < 0) b = -b;
            int gcd = GreatestCommonDivisor(a, b);                                                                      // Намира се НОД
            return (a * b) / gcd;                                                                                       // Произведението от числата се разделя на него и това е търсеното НОК
        }

        private int MaxNumberInRow(int[,] matrix, int row)                                                              // Намира най-голямаото число в даден ред от матрица
        {
            int max = matrix[row, 0];

            int colsCount = matrix.GetLength(1);
            for (int pos = 1; pos < colsCount; pos++)
            {
                if (matrix[row, pos] > max) max = matrix[row, pos];
            }

            return max;
        }
    }
}
