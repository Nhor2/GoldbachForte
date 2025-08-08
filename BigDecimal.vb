Imports System.Numerics

Public Class BigDecimal
    Private unscaledValue As BigInteger
    Private scale As Integer

    ' Costruttori e Primitive
    '  
    '                                                ____
    '     ___                                      .-~. /_"-._
    '    `-._~-.                                  / /_ "~o\  Y
    '        \  \                                / : \~x.  ` ')
    '         ]  Y                              /  |  Y< ~-.__j
    '        /   !                        _.--~T : l  l<  /.-~
    '       /   /                 ____.--~ .   ` l /~\ \<|Y
    '      /   /             .-~~"        /| .    ',-~\ \L|
    '     /   /             /     .^   \ Y~Y \.^>/l_   "--'
    '    /   Y           .-"(  .  l__  j_j l_/ /~_.-~    .
    '   Y    l          /    \  )    ~~~." / `/"~ / \.__/l_
    '   |     \     _.-"      ~-{__     l    l._Z~-.___.--~
    '   |      ~---~           /   ~~"---\_  ' __[>
    '   l  .                _.^   ___     _>-y~
    '    \  \     .      .-~   .-~   ~>--"  /
    '     \  ~---"            /     ./  _.-'
    '      "-.,_____.,_  _.--~\     _.-~
    '                  ~~     (   _}       -Row
    '                         `. ~(
    '                           )  \
    '                          /,`--'~\--'~\
    '                ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    '                           ->T-Rex<-

    Public Sub New(value As BigInteger, scale As Integer)
        Me.unscaledValue = value
        Me.scale = scale
    End Sub

    Public Sub New(value As Integer)
        Me.unscaledValue = New BigInteger(value)
        Me.scale = 0
    End Sub

    Public Sub New(value As Double)
        ' Converti il Double in stringa usando InvariantCulture
        Dim s As String = value.ToString(System.Globalization.CultureInfo.InvariantCulture)
        ' Chiama il costruttore da stringa
        InitializeFromString(s)
    End Sub

    ' Metodo helper privato per parsare la stringa
    Private Shared Function ParseString(value As String) As (unscaledValue As BigInteger, scale As Integer)
        Dim parts() As String = value.Split("."c)
        If parts.Length = 1 Then
            Return (BigInteger.Parse(parts(0)), 0)
        ElseIf parts.Length = 2 Then
            Dim scale = parts(1).Length
            Dim combined As String = parts(0) & parts(1)
            Return (BigInteger.Parse(combined), scale)
        Else
            Throw New FormatException("Formato stringa non valido.")
        End If
    End Function

    ' Metodo privato per inizializzare dai dati della stringa
    Private Sub InitializeFromString(value As String)
        Dim parsed = ParseString(value)
        Me.unscaledValue = parsed.unscaledValue
        Me.scale = parsed.scale
    End Sub

    Public Sub New(value As String)
        ' Costruttore da stringa "12345.6789"
        Dim parts() As String = value.Split("."c)
        If parts.Length = 1 Then
            ' Nessun decimale
            unscaledValue = BigInteger.Parse(parts(0))
            scale = 0
        ElseIf parts.Length = 2 Then
            scale = parts(1).Length
            Dim combined As String = parts(0) & parts(1)
            unscaledValue = BigInteger.Parse(combined)
        Else
            Throw New FormatException("Formato stringa non valido.")
        End If
    End Sub

    ' Override ToString per mostrare il valore decimale
    Public Overrides Function ToString() As String
        Dim s As String = unscaledValue.ToString()
        If scale = 0 Then Return s

        Dim isNegative As Boolean = False
        Dim strValue As String = s

        If s.StartsWith("-") Then
            isNegative = True
            strValue = s.Substring(1)
        End If

        ' Se il numero è più corto della scala, aggiungo zeri davanti
        If strValue.Length <= scale Then
            strValue = strValue.PadLeft(scale + 1, "0"c)
        End If

        Dim intPart As String = strValue.Substring(0, strValue.Length - scale)
        Dim decPart As String = strValue.Substring(strValue.Length - scale)

        Dim result As String = intPart & "." & decPart

        If isNegative Then
            result = "-" & result
        End If

        Return result.TrimEnd("0"c).TrimEnd("."c) ' Rimuove zeri finali e punto se necessario
    End Function

    ' Somma (più semplice)
    Public Shared Operator +(a As BigDecimal, b As BigDecimal) As BigDecimal
        Dim diffScale As Integer = a.scale - b.scale
        If diffScale = 0 Then
            Return New BigDecimal(a.unscaledValue + b.unscaledValue, a.scale)
        ElseIf diffScale > 0 Then
            Dim scaledB As BigInteger = b.unscaledValue * BigInteger.Pow(10, diffScale)
            Return New BigDecimal(a.unscaledValue + scaledB, a.scale)
        Else
            Dim scaledA As BigInteger = a.unscaledValue * BigInteger.Pow(10, -diffScale)
            Return New BigDecimal(scaledA + b.unscaledValue, b.scale)
        End If
    End Operator

    ' Sottrazione (idem)
    Public Shared Operator -(a As BigDecimal, b As BigDecimal) As BigDecimal
        Dim diffScale As Integer = a.scale - b.scale
        If diffScale = 0 Then
            Return New BigDecimal(a.unscaledValue - b.unscaledValue, a.scale)
        ElseIf diffScale > 0 Then
            Dim scaledB As BigInteger = b.unscaledValue * BigInteger.Pow(10, diffScale)
            Return New BigDecimal(a.unscaledValue - scaledB, a.scale)
        Else
            Dim scaledA As BigInteger = a.unscaledValue * BigInteger.Pow(10, -diffScale)
            Return New BigDecimal(scaledA - b.unscaledValue, b.scale)
        End If
    End Operator

    ' Moltiplicazione
    Public Shared Operator *(a As BigDecimal, b As BigDecimal) As BigDecimal
        Return New BigDecimal(a.unscaledValue * b.unscaledValue, a.scale + b.scale)
    End Operator

    ' Divisione semplice con scala fissa (troncamento)
    Public Shared Operator /(a As BigDecimal, b As BigDecimal) As BigDecimal
        Const defaultScale As Integer = 50 ' Precisione decimali

        Dim scaleDiff As Integer = defaultScale + b.scale - a.scale

        Dim dividend As BigInteger
        If scaleDiff >= 0 Then
            dividend = a.unscaledValue * BigInteger.Pow(10, scaleDiff)
        Else
            ' scaleDiff negativo: dividiamo il numeratore per 10^(-scaleDiff)
            Dim divisor As BigInteger = BigInteger.Pow(10, -scaleDiff)
            dividend = a.unscaledValue / divisor
        End If

        Dim quotient As BigInteger = dividend / b.unscaledValue
        Return New BigDecimal(quotient, defaultScale)
    End Operator

    ''' <summary>
    ''' Calcola la radice quadrata del valore corrente con la precisione specificata.
    ''' </summary>
    ''' <param name="precision">Numero di cifre decimali di precisione desiderata.</param>
    ''' <returns>Una nuova istanza di BigDecimal che rappresenta la radice quadrata.</returns>
    ''' <exception cref="ArithmeticException">Se il valore è negativo.</exception>
    Public Function Sqrt(precision As Integer) As BigDecimal
        'Radice 2
        If Me.unscaledValue.Sign < 0 Then Throw New ArithmeticException("Radice quadrata non definita per numeri negativi.")

        Dim scaleFactor As BigInteger = BigInteger.Pow(10, precision * 2)
        Dim scaledValue As BigInteger = Me.unscaledValue * scaleFactor

        Dim xPrev As BigInteger = scaledValue
        Dim x As BigInteger = scaledValue >> 1 ' Iniziamo con una stima

        Dim two As New BigDecimal(2)

        Dim iterations As Integer = 0
        Dim maxIterations As Integer = 1000

        While BigInteger.Abs(x - xPrev) > 1 And iterations < maxIterations
            xPrev = x
            x = (x + scaledValue / x) >> 1
            iterations += 1
        End While

        Return New BigDecimal(x, precision)
    End Function

    Public Function CompareTo(other As BigDecimal) As Integer
        ' Normalizza la scala per confronto
        Dim diffScale As Integer = Me.scale - other.scale
        If diffScale = 0 Then
            Return Me.unscaledValue.CompareTo(other.unscaledValue)
        ElseIf diffScale > 0 Then
            Dim scaledOther As BigInteger = other.unscaledValue * BigInteger.Pow(10, diffScale)
            Return Me.unscaledValue.CompareTo(scaledOther)
        Else
            Dim scaledSelf As BigInteger = Me.unscaledValue * BigInteger.Pow(10, -diffScale)
            Return scaledSelf.CompareTo(other.unscaledValue)
        End If
    End Function

    Public Function Equals(other As BigDecimal) As Boolean
        'Uguale
        Return CompareTo(other) = 0
    End Function


    ''' <summary>
    ''' Restituisce il valore assoluto del numero BigDecimal corrente.
    ''' </summary>
    ''' <returns>Una nuova istanza di BigDecimal con valore positivo equivalente al valore assoluto.</returns>
    Public Function Abs() As BigDecimal
        'Assoluto
        If Me.unscaledValue.Sign < 0 Then
            Return New BigDecimal(BigInteger.Negate(Me.unscaledValue), Me.scale)
        Else
            Return Me
        End If
    End Function

    ''' <summary>
    ''' Calcola il logaritmo in base 10 del valore corrente con la precisione specificata.
    ''' </summary>
    ''' <param name="precision">La precisione desiderata (numero di cifre decimali).</param>
    ''' <returns>Un nuovo BigDecimal che rappresenta il logaritmo base 10 del numero.</returns>
    ''' <exception cref="ArithmeticException">Se il valore è minore o uguale a zero.</exception>
    Public Function Log10(precision As Integer) As BigDecimal
        'Logaritmo
        Dim ln10 As BigDecimal = New BigDecimal("10").Ln(precision)
        Return Me.Ln(precision) / ln10
    End Function


    ''' <summary>
    ''' Calcola il logaritmo naturale (ln) del valore corrente con la precisione specificata.
    ''' </summary>
    ''' <param name="precision">La precisione desiderata (numero di cifre decimali).</param>
    ''' <returns>Un nuovo BigDecimal che rappresenta il logaritmo naturale del numero.</returns>
    ''' <exception cref="ArithmeticException">Se il valore è minore o uguale a zero.</exception>
    Public Function Ln(precision As Integer) As BigDecimal
        ' Natural logarithm: ln(x)
        ' Usa serie di Taylor per ln((x-1)/ (x+1)) per x vicino a 1
        ' ln(x) = 2 * sum_{n=0}^\infty [ ((x-1)/(x+1))^(2n+1) / (2n+1) ]

        If Me.unscaledValue.Sign <= 0 Then Throw New ArithmeticException("ln non definito per valori <= 0")

        Dim one As New BigDecimal(1)
        Dim x As BigDecimal = Me

        ' Trasformazione per portare x vicino a 1: ln(x) = ln(a * 10^k) = ln(a) + k*ln(10)
        Dim k As Integer = 0
        Dim ten As New BigDecimal(10)

        While x.CompareTo(ten) >= 0
            x = x / ten
            k += 1
        End While
        While x.CompareTo(one) < 0
            x = x * ten
            k -= 1
        End While

        Dim y As BigDecimal = (x - one) / (x + one)
        Dim yPower As BigDecimal = y
        Dim result As BigDecimal = y
        Dim n As Integer = 1

        Dim threshold As New BigDecimal(BigInteger.Pow(10, precision), precision)

        Do
            yPower = yPower * y * y
            Dim denom As BigDecimal = New BigDecimal(2 * n + 1)
            Dim term As BigDecimal = yPower / denom
            result += term
            n += 1

            If term.Abs().CompareTo(threshold) <= 0 OrElse n > 10000 Then Exit Do
        Loop

        Dim ln10 As BigDecimal = New BigDecimal(Math.Log(10)).Pow(1, precision) ' Approximate ln(10)
        Return result * New BigDecimal(2) + (New BigDecimal(k) * ln10)
    End Function


    ''' <summary>
    ''' Calcola l'esponenziale (e^x) del valore corrente con la precisione specificata.
    ''' </summary>
    ''' <param name="precision">La precisione desiderata (numero di cifre decimali).</param>
    ''' <returns>Un nuovo BigDecimal che rappresenta e elevato al valore corrente.</returns>
    Public Function Exp(precision As Integer) As BigDecimal
        'Esponenziale
        Dim one As New BigDecimal(1)
        Dim result As BigDecimal = one
        Dim term As BigDecimal = one
        Dim n As Integer = 1

        Dim threshold As New BigDecimal(BigInteger.Pow(10, precision), precision)

        Do
            term = term * Me / New BigDecimal(n)
            result += term
            n += 1
        Loop While term.Abs().CompareTo(threshold) > 0 AndAlso n < 1000

        Return result
    End Function


    ''' <summary>
    ''' Calcola la potenza del valore corrente elevato a un esponente intero.
    ''' </summary>
    ''' <param name="exponent">L'esponente intero a cui elevare il valore.</param>
    ''' <param name="precision">La precisione desiderata (numero di cifre decimali) per calcoli interni.</param>
    ''' <returns>Un nuovo BigDecimal che rappresenta il valore corrente elevato all'esponente.</returns>
    Public Function Pow(exponent As Integer, precision As Integer) As BigDecimal
        'Potenza
        If exponent = 0 Then Return New BigDecimal(1)
        If exponent < 0 Then
            Return (New BigDecimal(1) / Me.Pow(-exponent, precision))
        End If

        Dim result As BigDecimal = New BigDecimal(1)
        Dim baseVal As BigDecimal = Me
        Dim exp As Integer = exponent

        While exp > 0
            If (exp And 1) = 1 Then
                result = result * baseVal
            End If
            baseVal = baseVal * baseVal
            exp >>= 1
        End While

        Return result
    End Function


    ''' <summary>
    ''' Calcola l'arcotangente (arctan) di un valore BigDecimal usando una serie di Taylor.
    ''' </summary>
    ''' <param name="x">Il valore di input per cui calcolare l'arctan.</param>
    ''' <param name="precision">La precisione desiderata (numero di cifre decimali) per la convergenza della serie.</param>
    ''' <returns>Un nuovo BigDecimal che rappresenta l'arcotangente di x.</returns>
    Public Shared Function Arctan(x As BigDecimal, precision As Integer) As BigDecimal
        'Arc Tangente
        Dim xPower As BigDecimal = x
        Dim result As BigDecimal = x
        Dim term As BigDecimal = x
        Dim currentTerm As BigDecimal = x ' <-- dichiarato qui correttamente
        Dim n As Integer = 1
        Dim minusOne As New BigDecimal(-1)
        Dim threshold As New BigDecimal(BigInteger.Pow(10, precision), precision)

        Do
            term = term * x * x * minusOne
            Dim denom As BigDecimal = New BigDecimal(2 * n + 1)
            currentTerm = term / denom
            result += currentTerm
            n += 1
        Loop While currentTerm.Abs().CompareTo(threshold) > 0 AndAlso n < 10000

        Return result
    End Function


    ''' <summary>
    ''' Restituisce il valore opposto (negativo) del numero corrente.
    ''' </summary>
    ''' <returns>Un nuovo BigDecimal che rappresenta il valore negativo di questo numero.</returns>
    Public Function Negate() As BigDecimal
        'Negativo
        Return New BigDecimal(BigInteger.Negate(Me.unscaledValue), Me.scale)
    End Function


    ''' <summary>
    ''' Restituisce il valore massimo tra questo numero e un altro BigDecimal.
    ''' </summary>
    ''' <param name="b">Il BigDecimal da confrontare.</param>
    ''' <returns>Il BigDecimal con valore maggiore.</returns>
    Public Shared Function Max(a As BigDecimal, b As BigDecimal) As BigDecimal
        'Massimo
        If a.CompareTo(b) >= 0 Then
            Return a
        Else
            Return b
        End If
    End Function


    ''' <summary>
    ''' Restituisce il valore minimo tra questo numero e un altro BigDecimal.
    ''' </summary>
    ''' <param name="b">Il BigDecimal da confrontare.</param>
    ''' <returns>Il BigDecimal con valore minore.</returns>
    Public Shared Function Min(a As BigDecimal, b As BigDecimal) As BigDecimal
        'Minimo
        If a.CompareTo(b) <= 0 Then
            Return a
        Else
            Return b
        End If
    End Function

    Public Shared Operator Mod(a As BigDecimal, b As BigDecimal) As BigDecimal
        Dim division As BigDecimal = a / b

        ' Otteniamo la parte intera di division troncando la scala
        Dim scaleFactor As BigInteger = BigInteger.Pow(10, division.scale)
        Dim integerPart As BigInteger = division.unscaledValue / scaleFactor

        ' Calcoliamo il prodotto b * integerPart
        Dim product As BigDecimal = b * New BigDecimal(integerPart, 0)

        Return a - product
    End Operator


    ''' <summary>
    ''' Calcola il fattoriale di un numero intero non negativo.
    ''' </summary>
    ''' <param name="n">Numero intero non negativo di cui calcolare il fattoriale.</param>
    ''' <returns>Il fattoriale di n come BigInteger.</returns>
    ''' <exception cref="ArgumentException">Se n è negativo.</exception>
    Public Shared Function Factorial(n As Integer) As BigInteger
        'Fattoriale
        Dim result As BigInteger = BigInteger.One
        For i As Integer = 2 To n
            result *= i
        Next
        Return result
    End Function


    ''' <summary>
    ''' Calcola il seno del valore corrente (in radianti) usando la serie di Taylor.
    ''' </summary>
    ''' <param name="precision">Precisione decimale desiderata (numero di cifre decimali).</param>
    ''' <returns>Valore del seno come BigDecimal con la precisione specificata.</returns>
    Public Function Sin(precision As Integer) As BigDecimal
        'Seno
        Dim x As BigDecimal = Me
        Dim term As BigDecimal = x ' primo termine x^1/1!
        Dim result As BigDecimal = term
        Dim n As Integer = 1
        Dim minusOne As New BigDecimal(-1)
        Dim threshold As New BigDecimal(BigInteger.One, precision)

        Do
            Dim powerIndex As Integer = 2 * n + 1
            term = (term * x * x * minusOne) / New BigDecimal(powerIndex * (powerIndex - 1))
            result += term
            n += 1
        Loop While term.Abs().CompareTo(threshold) > 0 AndAlso n < 10000

        Return result
    End Function


    ''' <summary>
    ''' Calcola il coseno del valore corrente (in radianti) usando la serie di Taylor.
    ''' </summary>
    ''' <param name="precision">Precisione decimale desiderata (numero di cifre decimali).</param>
    ''' <returns>Valore del coseno come BigDecimal con la precisione specificata.</returns>
    Public Function Cos(precision As Integer) As BigDecimal
        'Coseno
        Dim x As BigDecimal = Me
        Dim term As BigDecimal = New BigDecimal(1) ' primo termine 1
        Dim result As BigDecimal = term
        Dim n As Integer = 1
        Dim minusOne As New BigDecimal(-1)
        Dim threshold As New BigDecimal(BigInteger.One, precision)

        Do
            Dim powerIndex As Integer = 2 * n
            term = (term * x * x * minusOne) / New BigDecimal(powerIndex * (powerIndex - 1))
            result += term
            n += 1
        Loop While term.Abs().CompareTo(threshold) > 0 AndAlso n < 10000

        Return result
    End Function


    ''' <summary>
    ''' Calcola la tangente del valore corrente (in radianti) usando il rapporto tra seno e coseno.
    ''' </summary>
    ''' <param name="precision">Precisione decimale desiderata (numero di cifre decimali).</param>
    ''' <returns>Valore della tangente come BigDecimal con la precisione specificata.</returns>
    ''' <exception cref="ArithmeticException">Se il coseno è zero, genera un errore di divisione per zero.</exception>
    Public Function Tan(precision As Integer) As BigDecimal
        'Tangente
        Dim cosVal As BigDecimal = Me.Cos(precision + 5) ' un po' di margine precisione
        Dim sinVal As BigDecimal = Me.Sin(precision + 5)
        Return sinVal / cosVal
    End Function

    ' Fine Primitive



    ' Help
    'Dim rho As Double = SezioneArgentea()
    'Debug.WriteLine("Sezione: " & rho.ToString("F50"))

    'Dim sezione As BigDecimal = BigDecimal.SezioneArgentea(20, 50)
    'Debug.WriteLine("Sezione Big: " & sezione.ToString())

    'Dim pig As BigDecimal = BigDecimal.PiGreco(50)
    'Debug.WriteLine("PiGreco Big: " & pig.ToString())

    'Dim precision As Integer = 50

    'Dim num1 As New BigDecimal("2")
    'Dim sqrt2 As BigDecimal = num1.Sqrt(precision)
    'Dim ln2 As BigDecimal = num1.Ln(precision)

    'Dim num2 As New BigDecimal("10")
    'Dim sqrt10 As BigDecimal = num2.Sqrt(precision)
    'Dim ln10 As BigDecimal = num2.Ln(precision)

    'Console.WriteLine("Sqrt(2) = " & sqrt2.ToString())
    'Console.WriteLine("Ln(2) = " & ln2.ToString())

    'Console.WriteLine("Sqrt(10) = " & sqrt10.ToString())
    'Console.WriteLine("Ln(10) = " & ln10.ToString())

    'Dim x As New BigDecimal("1.4142135623") ' circa sqrt(2)
    'Dim pow2 As BigDecimal = x.Pow(2, 50)
    'Console.WriteLine("x^2 = " & pow2.ToString()) ' dovrebbe essere circa 2.0

    'Dim a As New BigDecimal("10.75")
    'Dim b As New BigDecimal("3.2")

    'Dim result As BigDecimal = a Mod b
    'Console.WriteLine("Modulo di " & a.ToString() & " % " & b.ToString() & " = " & result.ToString())

    'precision = 30 ' numero di cifre decimali di precisione

    ' Angolo in radianti (ad esempio pi/4 ~ 0.785398...)
    'Dim pi As BigDecimal = BigDecimal.PiGreco(precision + 10) ' Prendi pi con precisione extra
    'Dim angle As BigDecimal = pi / New BigDecimal(4) ' pi/4 radianti

    'Console.WriteLine("Angolo (pi/4): " & angle.ToString())

    'Dim sinVal As BigDecimal = angle.Sin(precision)
    'Dim cosVal As BigDecimal = angle.Cos(precision)
    'Dim tanVal As BigDecimal = angle.Tan(precision)

    'Console.WriteLine("Sin(pi/4) = " & sinVal.ToString())
    'Console.WriteLine("Cos(pi/4) = " & cosVal.ToString())
    'Console.WriteLine("Tan(pi/4) = " & tanVal.ToString())

    ' Test fattoriale (ad esempio 10!)
    'Dim fact10 As BigInteger = BigDecimal.Factorial(10)
    'Console.WriteLine("10! = " & fact10.ToString())


    ' Sezione Costanti
    Public Shared Function SezioneArgentea(iterazioni As Integer, precisione As Integer) As BigDecimal
        Dim due As New BigDecimal(2)
        Dim x As New BigDecimal(2) ' valore iniziale

        For i As Integer = 1 To iterazioni
            x = due + (New BigDecimal(1) / x)
        Next

        Return x
    End Function

    Public Shared Function Phi(precision As Integer) As BigDecimal
        ' Phi = (1 + sqrt(5)) / 2
        Dim five As New BigDecimal("5")
        Dim sqrt5 As BigDecimal = five.Sqrt(precision)
        Dim one As New BigDecimal("1")
        Dim two As New BigDecimal("2")

        Return (one + sqrt5) / two
    End Function

    Public Shared Function NumeroPlastico(precision As Integer) As BigDecimal
        ' Risolve x^3 = x + 1 usando Newton
        Dim x As BigDecimal = New BigDecimal("1")
        Dim one As New BigDecimal("1")
        Dim three As New BigDecimal("3")
        Dim epsilon As New BigDecimal(BigInteger.Pow(10, precision), precision)

        Dim iteration As Integer = 0
        Do
            Dim fx As BigDecimal = x * x * x - x - one
            Dim fpx As BigDecimal = three * x * x - New BigDecimal("1")
            Dim xNew As BigDecimal = x - fx / fpx
            Dim diff As BigDecimal = (xNew - x).Abs()

            x = xNew
            iteration += 1

            If diff.CompareTo(epsilon) <= 0 OrElse iteration > 1000 Then Exit Do
        Loop

        Return x
    End Function

    Public Shared Function PiGreco(precision As Integer) As BigDecimal
        Dim one As New BigDecimal(1)
        Dim five As New BigDecimal(5)
        Dim twoThreeNine As New BigDecimal(239)
        Dim four As New BigDecimal(4)

        Dim arctan1_5 As BigDecimal = Arctan(one / five, precision)
        Dim arctan1_239 As BigDecimal = Arctan(one / twoThreeNine, precision)

        Dim pi As BigDecimal = four * (four * arctan1_5 - arctan1_239)
        Return pi
    End Function

    Public Shared Function SilverRatio(precision As Integer) As BigDecimal
        ' Silver Ratio = 1 + sqrt(2)
        Dim one As New BigDecimal(1, precision)
        Dim two As New BigDecimal(2, precision)
        Dim sqrtTwo As BigDecimal = two.Sqrt(precision + 5)
        Return one + sqrtTwo
    End Function
End Class