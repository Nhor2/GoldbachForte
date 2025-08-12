Imports System.ComponentModel
Imports System.Numerics

Public Class Form1

    Dim numero As Integer = 0
    Dim divisori As List(Of Integer)

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        divisori = New List(Of Integer)

        ComboBox1.SelectedIndex = 0

        'Log Goldbach Forte
        Dim path As String = System.IO.Path.Combine(Application.StartupPath, "Goldbach_forte.txt")

        If System.IO.File.Exists(path) Then
            Try
                ' Leggi le ultime 100 righe (in modo sicuro)
                Dim allLines = System.IO.File.ReadLines(path).Reverse().Take(100)

                'powershell -Command "Get-Content Goldbach_forte.txt -Tail 100"


                For Each line In allLines
                    ' Trova la prima riga che sembra un risultato numerico
                    If line.StartsWith("[") AndAlso line.Contains("]:") Then
                        ' Estrai il numero tra le parentesi quadre
                        Dim startBracket = line.IndexOf("[") + 1
                        Dim endBracket = line.IndexOf("]")
                        Dim numberString = line.Substring(startBracket, endBracket - startBracket)

                        Dim lastNumber As BigInteger = 0
                        If BigInteger.TryParse(numberString, lastNumber) Then
                            ' Imposta il TextBox con il prossimo numero pari
                            Dim nextNumber As BigInteger = If(lastNumber Mod 2 = 0, lastNumber + 2, lastNumber + 1)
                            TextBoxRange2.Text = nextNumber.ToString()
                            LabelAutoStart.Visible = True
                        End If
                        Exit For ' Fermati dopo la prima riga valida
                    End If
                Next
            Catch ex As Exception
                ' Puoi loggare o ignorare l'errore
            End Try
        End If
    End Sub

    Private Sub Form1_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        RichTextBox2.Width = CInt(Me.Width - RichTextBox2.Location.X - (RichTextBox1.Location.X * 2))
        RichTextBox3.Width = CInt(RichTextBox2.Width - RichTextBox2.Location.X)
        RichTextBox4.Width = CInt(RichTextBox2.Width - RichTextBox2.Location.X)
    End Sub

    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged
        numero = CInt(Val(TextBox1.Text))

        If Not Integer.TryParse(TextBox1.Text, numero) Then
            RichTextBox1.Clear()
            RichTextBox2.Text = "Inserisci un numero intero valido"
            Exit Sub
        End If

        calcolo()
    End Sub

    Private Sub calcolo()
        Dim num As Integer = numero
        RichTextBox1.Clear()
        divisori.Clear()
        RichTextBox2.Clear()

        'Divisori
        For q = 1 To num
            If num Mod q = 0 Then
                divisori.Add(q)
            End If
        Next

        For Each divisore In divisori
            RichTextBox1.Text &= num.ToString & " é divisibile per " & divisore.ToString & vbCrLf
        Next

        'Fattoriali
        If num <= 500 Then
            Dim fattorialeList As String = num.ToString()
            For w = num - 1 To 1 Step -1
                fattorialeList &= " * " & w.ToString
            Next
            Dim risultatoFattoiale As BigInteger = Fattoriale(num)

            RichTextBox2.Text &= num.ToString & "! = " & risultatoFattoiale.ToString & " (" & num.ToString & fattorialeList & ")" & vbCrLf
        End If

        'Radice
        RichTextBox2.Text &= "Radice(2) " & num.ToString & " = " & Math.Sqrt(num).ToString & vbCrLf

        'Quadrato
        RichTextBox2.Text &= "Quadrato " & num.ToString & " ^ 2  = " & (num * num).ToString & vbCrLf

        'Cubo
        RichTextBox2.Text &= "Cubo " & num.ToString & " ^ 3  = " & (num * num * num).ToString & vbCrLf

        'Seno
        RichTextBox2.Text &= "Seno Sin( " & num.ToString & ") = " & (Math.Sin(num)).ToString & vbCrLf

        'CoSeno
        RichTextBox2.Text &= "Coseno Cos( " & num.ToString & ") = " & (Math.Cos(num)).ToString & vbCrLf

        'Silver Ratio
        Dim silverRatio As BigDecimal = BigDecimal.SilverRatio(num)
        RichTextBox2.Text &= "Silver Rario(" & num.ToString & ") = " & silverRatio.ToString & vbCrLf

        'Primo
        If EPrimo(num) Then
            RichTextBox2.Text &= num.ToString & " è un numero PRIMO: ha solo 2 divisori: 1 e " & num.ToString & vbCrLf
        Else
            RichTextBox2.Text &= num.ToString & " NON è un numero primo: ha " & divisori.Count & " divisori" & vbCrLf
        End If

        'Perfetto
        If EPerfetto(num) Then
            RichTextBox2.Text &= num.ToString & " è un numero perfetto!" & vbCrLf
        End If

        'Pari
        If num Mod 2 = 0 Then
            RichTextBox2.Text &= num.ToString & " è un numero PARI" & vbCrLf
        Else
            RichTextBox2.Text &= num.ToString & " è un numero DISPARI" & vbCrLf
        End If

        'Palindromo
        If EPalindromo(num) Then
            RichTextBox2.Text &= num.ToString & " è un numero palindromo" & vbCrLf
        End If

        'Fibonacci
        If EFibonacci(num) Then
            RichTextBox2.Text &= num.ToString & " fa parte della sequenza di Fibonacci" & vbCrLf
        End If

        'QuadratoPerfetto
        If EQuadratoPerfetto(num) Then
            RichTextBox2.Text &= num.ToString & " é un Quadrato Perfetto" & vbCrLf
        Else
            RichTextBox2.Text &= num.ToString & " NON é un Quadrato Perfetto" & vbCrLf
        End If

        'Harshad
        If EHarshad(num) Then
            RichTextBox2.Text &= num.ToString & " é un numero Harshad divisibile per la somma delle sue cifre" & vbCrLf
        Else
            RichTextBox2.Text &= num.ToString & " NON é un numero Harshad" & vbCrLf
        End If

        'Kaprekar
        If EKaprekar(num) Then
            RichTextBox2.Text &= num.ToString() & " è un numero Kaprekar" & vbCrLf
        End If

        'Narcisistico
        If EArmstrong(num) Then
            RichTextBox2.Text &= num.ToString() & " è un numero Armstrong (o Narcisistico)" & vbCrLf
        End If
    End Sub


    Function EArmstrong(n As Integer) As Boolean
        'numero Narcisistico o di Armstrong
        Dim s As String = n.ToString()
        Dim somma As Integer = 0
        Dim cifre As Integer = s.Length
        For Each c As Char In s
            somma += CInt(Math.Pow(CInt(c.ToString()), cifre))
        Next
        Return somma = n
    End Function


    Function GoldbachDebole(n As Integer) As String
        'Ogni numero dispari maggiore di 5 può essere scritto come somma di tre numeri primi.
        'Congettura debole: dimostrata nel 2013 da Harald Helfgott, quindi ora è un teorema.
        If n <= 5 Or n Mod 2 = 0 Then
            Return "Numero non valido per la congettura di Goldbach debole (serve un dispari > 5)"
        End If

        For i As Integer = 2 To n
            If Not EPrimo(i) Then Continue For
            For j As Integer = 2 To n
                If Not EPrimo(j) Then Continue For
                For k As Integer = 2 To n
                    If Not EPrimo(k) Then Continue For
                    If i + j + k = n Then
                        Return $"{n} = {i} + {j} + {k}"
                    End If
                Next
            Next
        Next

        Return "Nessuna terna di primi trovata"
    End Function



    Function GoldbachForte(n As Integer) As String
        'Ogni numero pari maggiore di 2 può essere scritto come somma di due numeri primi.
        If n <= 2 Or n Mod 2 <> 0 Then
            Return "Numero non valido per la congettura di Goldbach forte"
        End If

        For i = 2 To n \ 2
            If EPrimo(i) AndAlso EPrimo(n - i) Then
                Return n.ToString() & " = " & i.ToString() & " + " & (n - i).ToString()
            End If
        Next

        Return "Nessuna coppia di primi trovata (impossibile?)"
    End Function


    Function GoldbachForteBig(n As BigInteger) As String
        ' Verifica che sia un numero pari > 2
        If n <= 2 OrElse n Mod 2 <> 0 Then
            Return "Numero non valido per la congettura di Goldbach forte"
        End If

        'Dim half As BigInteger = n / 2
        Dim half As BigInteger = BigInteger.Divide(n, 2)

        For i As BigInteger = 2 To half
            If EPrimoBig(i) AndAlso EPrimoBig(n - i) Then
                Return $"{n} = {i} + {n - i}"
            End If
        Next

        Return "Nessuna coppia di primi trovata (impossibile?)"
    End Function


    Function EPrimoBig(n As BigInteger) As Boolean
        If n < 2 Then Return False
        If n = 2 Then Return True
        If n Mod 2 = 0 Then Return False

        Dim limite As BigInteger = BigIntegerSqrt(n)
        For i As BigInteger = 3 To limite Step 2
            If n Mod i = 0 Then Return False
        Next

        Return True
    End Function

    Function BigIntegerSqrt(n As BigInteger) As BigInteger
        If n < 0 Then Throw New ArgumentException("Radice quadrata di un numero negativo non è definita.")
        If n = 0 OrElse n = 1 Then Return n

        Dim low As BigInteger = 0
        Dim high As BigInteger = n
        Dim mid As BigInteger

        While low <= high
            mid = BigInteger.Divide(low + high, 2) '(low + high) \ 2
            Dim midSquared As BigInteger = mid * mid

            If midSquared = n Then
                Return mid
            ElseIf midSquared < n Then
                low = mid + 1
            Else
                high = mid - 1
            End If
        End While

        Return high ' La radice intera più vicina (tronca i decimali)
    End Function



    Function EKaprekar(n As Integer) As Boolean
        'Un numero è Kaprekar se il quadrato del numero può essere diviso in due parti che sommate danno il numero stesso.
        If n = 1 Then Return True
        Dim square As String = (n * n).ToString()
        For i = 1 To square.Length - 1
            Dim left = CInt(square.Substring(0, i))
            Dim right = CInt(square.Substring(i))
            If right > 0 AndAlso left + right = n Then
                Return True
            End If
        Next
        Return False
    End Function


    Function EHarshad(n As Integer) As Boolean
        'divisibile per la somma delle sue cifre
        Dim somma As Integer = 0
        For Each c As Char In n.ToString()
            somma += CInt(c.ToString())
        Next
        Return n Mod somma = 0
    End Function

    Function EQuadratoPerfetto(n As Integer) As Boolean
        Dim radice As Integer = Math.Sqrt(n)
        Return radice * radice = n
    End Function


    Function EFibonacci(n As Integer) As Boolean
        Dim a As Integer = 0
        Dim b As Integer = 1

        While b < n
            Dim temp As Integer = a + b
            a = b
            b = temp
        End While

        Return b = n OrElse n = 0
    End Function


    Function EPalindromo(n As Integer) As Boolean
        Dim s As String = n.ToString()
        Dim reversed As String = StrReverse(s)
        Return s = reversed
    End Function

    Function Fattoriale(n As Integer) As BigInteger
        Dim result As BigInteger = 1
        For i As Integer = 2 To n
            result *= i
        Next
        Return result
    End Function

    Function EPrimo(n As Integer) As Boolean
        If n < 2 Then Return False
        If n = 2 Then Return True
        If n Mod 2 = 0 Then Return False

        Dim limite As Integer = Math.Sqrt(n)
        For i As Integer = 3 To limite Step 2
            If n Mod i = 0 Then
                Return False
            End If
        Next

        Return True
    End Function

    Function EPerfetto(n As Integer) As Boolean
        Dim somma As Integer = 0
        For Each d In divisori
            If d <> n Then
                somma += d
            End If
        Next
        Return somma = n
    End Function

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        'Calcolo con combobox
        Dim num As Integer = numero
        RichTextBox3.Clear()

        If ComboBox1.SelectedItem Is Nothing Then
            MessageBox.Show("Seleziona un'opzione dal menu a tendina.", "Attenzione", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        If ComboBox1.SelectedItem = "Goldbach forte (la forma originale di Goldbach, 1742)" Then
            If num > 2 AndAlso num Mod 2 = 0 Then
                RichTextBox3.Text &= "Congettura di Goldbach forte: " & GoldbachForte(num) & vbCrLf
            End If
        End If

        If ComboBox1.SelectedItem = "Goldbach debole (forma ternaria, ogni dispari > 5 = somma di 3 primi)" Then
            If num > 5 AndAlso num Mod 2 = 1 Then
                RichTextBox3.Text &= "Congettura di Goldbach debole: " & GoldbachDebole(num) & vbCrLf
            Else
                RichTextBox3.Text &= "Il numero deve essere dispari e maggiore di 5." & vbCrLf
            End If
        End If
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If BackgroundWorker1.IsBusy Then
            MessageBox.Show("Il calcolo è già in corso.")
            Return
        End If

        Dim input As String = TextBoxRange.Text.Trim()
        If Not input.Contains(",") Then
            MessageBox.Show("Inserisci due numeri separati da virgola, esempio: 34, 94")
            Return
        End If

        Dim parts() As String = input.Split(","c)
        If parts.Length <> 2 Then
            MessageBox.Show("Inserisci esattamente due numeri.")
            Return
        End If

        Dim n1, n2 As Integer
        If Not Integer.TryParse(parts(0).Trim(), n1) OrElse Not Integer.TryParse(parts(1).Trim(), n2) Then
            MessageBox.Show("Valori non validi.")
            Return
        End If

        If n1 > n2 Then
            Dim temp = n1
            n1 = n2
            n2 = temp
        End If

        RichTextBox3.Clear()
        ProgressBar1.Value = 0

        BackgroundWorker1.RunWorkerAsync(New Integer() {n1, n2})
    End Sub

    Private Function Goldbach2Forte(n As Integer) As String
        If n <= 2 Or n Mod 2 <> 0 Then
            Return "Non applicabile"
        End If

        For i As Integer = 2 To n \ 2
            If EPrimo(i) AndAlso EPrimo(n - i) Then
                Return $"{n} = {i} + {n - i}"
            End If
        Next

        Return "Nessuna coppia trovata"
    End Function


    'Calcolo
    Private Sub BackgroundWorker1_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker1.DoWork
        Dim range() As Integer = CType(e.Argument, Integer())
        Dim n1 As Integer = range(0)
        Dim n2 As Integer = range(1)
        Dim total As Integer = n2 - n1 + 1
        Dim done As Integer = 0

        For num As Integer = n1 To n2
            If BackgroundWorker1.CancellationPending Then
                e.Cancel = True
                Exit For
            End If

            If num > 2 AndAlso num Mod 2 = 0 Then
                Dim result As String = GoldbachForte(num)
                BackgroundWorker1.ReportProgress(CInt(((done / total) * 100)), $"[{num}]: {result}")
            End If

            done += 1
        Next
    End Sub

    Private Sub BackgroundWorker1_ProgressChanged(sender As Object, e As ProgressChangedEventArgs) Handles BackgroundWorker1.ProgressChanged
        ProgressBar1.Value = Math.Min(e.ProgressPercentage, 100)
        RichTextBox3.AppendText(e.UserState.ToString() & vbCrLf)
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        'Annulla il calcolo del BackgroundWorker
        If BackgroundWorker1.IsBusy Then
            BackgroundWorker1.CancelAsync()
        End If
    End Sub

    Private Sub BackgroundWorker1_RunWorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs) Handles BackgroundWorker1.RunWorkerCompleted
        If e.Cancelled Then
            RichTextBox3.AppendText(vbCrLf & "Operazione annullata." & vbCrLf)
        ElseIf e.Error IsNot Nothing Then
            RichTextBox3.AppendText(vbCrLf & "Errore durante il calcolo: " & e.Error.Message & vbCrLf)
        Else
            RichTextBox3.AppendText(vbCrLf & "Completato!" & vbCrLf)
            ProgressBar1.Value = 100

            ' Qui salviamo su file
            Dim path As String = System.IO.Path.Combine(Application.StartupPath, "Goldbach.txt")
            Try
                ' AppendText apre il file in append mode
                Dim separator As String = $"--- Calcolo eseguito il {DateTime.Now} ---{Environment.NewLine}"
                System.IO.File.AppendAllText(path, separator & RichTextBox3.Text & Environment.NewLine)
                MessageBox.Show($"Risultati aggiunti al file: {path}", "Salvataggio completato")
            Catch ex As Exception
                MessageBox.Show("Errore durante il salvataggio: " & ex.Message, "Errore")
            End Try
        End If
    End Sub




    'Calcolo Batch Continuo

    Private batchSize As Integer = 1000
    Private currentStart As BigInteger = 2 ' inizializza da 2 o da input
    Private CalcStop As Boolean = False ' stoppa il calcolo continuo
    Private counterBatch As Integer = 0

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        'Calcolo Batch
        If BackgroundWorker2.IsBusy Then
            MessageBox.Show("Calcolo continuo già in esecuzione.")
            Return
        End If

        ' Leggi input, minimo 2
        Dim input As String = TextBoxRange2.Text.Trim()
        Dim n1 As BigInteger = 0
        If Not BigInteger.TryParse(input, n1) Then
            MessageBox.Show("Inserisci un numero intero valido da cui partire.")
            Return
        End If

        If n1 > 2 Then
            currentStart = n1
        Else
            currentStart = 2
        End If

        'currentStart = Math.Max(2, n1)
        ProgressBar1.Value = 0
        RichTextBox4.Clear() ' Pulisce all'avvio del nuovo batch

        BackgroundWorker2.RunWorkerAsync(currentStart)
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        'Annulla continuo
        CalcStop = True 'Stronca
        If BackgroundWorker2.IsBusy Then
            BackgroundWorker2.CancelAsync()
        End If

        'Rilascio tutte le risorse
        GC.Collect()
        GC.WaitForPendingFinalizers()
        GC.Collect()
    End Sub

    Private Sub BackgroundWorker2_DoWork(sender As Object, e As DoWorkEventArgs) Handles BackgroundWorker2.DoWork
        Dim startNum As BigInteger = CType(e.Argument, BigInteger)
        Dim endNum As BigInteger = startNum + batchSize - 1
        Dim worker As BackgroundWorker = CType(sender, BackgroundWorker)
        Dim results As New System.Text.StringBuilder()

        For num As BigInteger = startNum To endNum
            If worker.CancellationPending Then
                e.Cancel = True
                Exit For
            End If

            If num > 2 AndAlso num Mod 2 = 0 Then
                'Dim res As String = GoldbachForte(num) 'Integer
                'Trasformiamo in BigInteger per non fermarci mai!!!
                Dim bigNum As BigInteger = num ' New BigInteger(num)
                Dim res As String = GoldbachForteBig(bigNum) 'BigInteger
                results.AppendLine($"[{num}]: {res}")
            End If

            Dim progress As BigInteger = CInt(((num - startNum + 1) / batchSize) * 100)
            worker.ReportProgress(progress, $"[{num}] calcolato")
        Next

        ' Salvataggio su file
        Dim path As String = System.IO.Path.Combine(Application.StartupPath, "Goldbach_forte.txt")
        Try
            Dim separator As String = $"--- Calcolo batch {counterBatch} da {startNum} a {endNum} eseguito il {DateTime.Now} ---{Environment.NewLine}"
            Using writer As New System.IO.StreamWriter(path, True)
                writer.WriteLine(separator)
                writer.Write(results.ToString())
                writer.WriteLine()
            End Using
        Catch ex As Exception
            ' Ignora errori file
        End Try

        ' Ritorna il prossimo numero da processare
        e.Result = endNum + 1
    End Sub

    Private Sub BackgroundWorker2_ProgressChanged(sender As Object, e As ProgressChangedEventArgs) Handles BackgroundWorker2.ProgressChanged
        ProgressBar1.Value = e.ProgressPercentage
        ' Se vuoi puoi aggiornare una label con e.UserState.ToString()
    End Sub

    Private Sub BackgroundWorker2_RunWorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs) Handles BackgroundWorker2.RunWorkerCompleted
        If CalcStop Then
            BackgroundWorker2.CancelAsync()
            Return
        End If

        If e.Cancelled Then
            RichTextBox4.AppendText(vbCrLf & "Calcolo continuo annullato." & vbCrLf)
        ElseIf e.Error IsNot Nothing Then
            RichTextBox4.AppendText(vbCrLf & "Errore nel calcolo continuo: " & e.Error.Message & vbCrLf)
        Else
            Dim msg As String = $"Batch {counterBatch} completato." & vbCrLf
            RichTextBox4.Text = msg & RichTextBox4.Text
            counterBatch += 1

            LimitRichTextBoxLines(RichTextBox4, 1000)
        End If

        ProgressBar1.Value = 0

        If Not e.Cancelled AndAlso e.Error Is Nothing Then
            ' Prendi prossimo start da e.Result e rilancia BackgroundWorker
            Dim nextStart As BigInteger = CType(e.Result, BigInteger)
            currentStart = nextStart

            ' Pulisci un po’ il RichTextBox4 per evitare crescita eccessiva (es. ultime 1000 righe)
            LimitRichTextBoxLines(RichTextBox4, 1000)

            ' Delay per dare tempo all’utente di premere “Annulla”
            Task.Delay(CInt(NumericUpDownPause.Value)).ContinueWith(Sub()
                                                                        If CalcStop = True Then BackgroundWorker2.CancelAsync()

                                                                        If Not BackgroundWorker2.CancellationPending Then
                                                                            If Not BackgroundWorker2.IsBusy Then
                                                                                BackgroundWorker2.RunWorkerAsync(currentStart)
                                                                            End If
                                                                        End If
                                                                    End Sub, TaskScheduler.FromCurrentSynchronizationContext())
        End If
    End Sub

    Private Sub LimitRichTextBoxLines(rtb As RichTextBox, maxLines As Integer)
        If rtb.Lines.Length > maxLines Then
            rtb.Clear()
        End If
    End Sub

End Class
