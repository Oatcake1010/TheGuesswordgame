Imports System.Text
Imports System.Windows.Forms
Imports System.Drawing
Imports System.Collections.Generic
Imports System.Media ' Import for SoundPlayer

Public Class Form1
    Inherits Form

    ' Declare the controls at class level
    Private lblWord As New Label()
    Private txtAnswer As New TextBox()
    Private lblResult As New Label()
    Private lblCorrectWord As New Label() ' New label to show the correct word
    Private WithEvents btnNext As New Button()
    Private WithEvents btnStart As New Button()
    Private WithEvents Timer1 As New Timer()

    ' Other declarations
    Dim currentWord As String
    Dim score As Integer = 0
    Dim rounds As Integer = 10
    Dim currentRound As Integer = 1
    Dim random As New Random()
    Dim difficulty As String

    ' Word lists based on difficulty level
    Dim easyWords As List(Of String) = New List(Of String) From {"cat", "dog", "fish", "ball", "tree", "book", "sun"}
    Dim mediumWords As List(Of String) = New List(Of String) From {"animal", "banana", "garden", "pencil", "flower", "planet", "window"}
    Dim hardWords As List(Of String) = New List(Of String) From {"elephant", "mountain", "astronomy", "adventure", "technology", "universe", "excellence"}

    ' Initialize the Form
    Public Sub New()
        Me.Text = "Word Game"
        Me.Size = New Size(350, 450)
        Me.BackColor = Color.White

        ' Initialize lblWord for displaying the word or hint
        lblWord.Text = "Word"
        lblWord.Location = New Point(50, 50)
        lblWord.Size = New Size(250, 40)
        lblWord.BackColor = Color.LightGray
        lblWord.Font = New Font("Arial", 16, FontStyle.Bold)
        lblWord.TextAlign = ContentAlignment.MiddleCenter
        Me.Controls.Add(lblWord)

        ' Initialize txtAnswer for user input
        txtAnswer.Location = New Point(50, 110)
        txtAnswer.Size = New Size(250, 40)
        txtAnswer.Font = New Font("Arial", 16)
        txtAnswer.TextAlign = HorizontalAlignment.Center
        Me.Controls.Add(txtAnswer)

        ' Initialize btnNext to go to the next word
        btnNext.Text = "Next"
        btnNext.Location = New Point(50, 170)
        btnNext.Size = New Size(100, 40)
        btnNext.Font = New Font("Arial", 14)
        btnNext.BackColor = Color.LightSkyBlue
        Me.Controls.Add(btnNext)

        ' Initialize btnStart to start the game
        btnStart.Text = "Start"
        btnStart.Location = New Point(200, 170)
        btnStart.Size = New Size(100, 40)
        btnStart.Font = New Font("Arial", 14)
        btnStart.BackColor = Color.LightSkyBlue
        Me.Controls.Add(btnStart)

        ' Initialize lblResult for displaying the result (Correct/Wrong)
        lblResult.Location = New Point(50, 230)
        lblResult.Size = New Size(250, 40)
        lblResult.Font = New Font("Arial", 16)
        lblResult.TextAlign = ContentAlignment.MiddleCenter
        lblResult.BackColor = Color.Transparent
        Me.Controls.Add(lblResult)

        ' Initialize lblCorrectWord to display the correct word after guessing
        lblCorrectWord.Location = New Point(50, 290)
        lblCorrectWord.Size = New Size(250, 40)
        lblCorrectWord.Font = New Font("Arial", 14)
        lblCorrectWord.TextAlign = ContentAlignment.MiddleCenter
        lblCorrectWord.BackColor = Color.LightYellow
        lblCorrectWord.Text = "" ' Initially empty
        Me.Controls.Add(lblCorrectWord)

        ' Start the game on button click
        AddHandler btnStart.Click, AddressOf btnStart_Click
        AddHandler btnNext.Click, AddressOf btnNext_Click
        AddHandler Timer1.Tick, AddressOf Timer1_Tick
    End Sub

    ' Main entry point for the application
    <STAThread()>
    Public Shared Sub Main()
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)
        Application.Run(New Form1())
    End Sub

    ' Function to select a random word based on the difficulty level
    Private Function GetRandomWord() As String
        Select Case difficulty
            Case "Easy"
                Return easyWords(random.Next(easyWords.Count))
            Case "Medium"
                Return mediumWords(random.Next(mediumWords.Count))
            Case "Hard"
                Return hardWords(random.Next(hardWords.Count))
            Case Else
                Return "Error" ' Default, should not happen
        End Select
    End Function

    ' Function to partially replace letters in the word with underscores
    Private Function PartiallyBlankWord(ByVal word As String) As String
        Dim chars As Char() = word.ToCharArray()
        Dim numBlanks As Integer

        ' Determine the number of blanks based on difficulty
        Select Case difficulty
            Case "Easy"
                numBlanks = Math.Min(1, word.Length \ 2)  ' Fewer blanks for easy
            Case "Medium"
                numBlanks = Math.Max(1, word.Length \ 2)  ' Moderate number of blanks
            Case "Hard"
                numBlanks = random.Next(word.Length \ 2, word.Length)  ' Most blanks for hard
        End Select

        ' Randomly replace letters with blanks
        Dim blankPositions As New HashSet(Of Integer)
        While blankPositions.Count < numBlanks
            blankPositions.Add(random.Next(word.Length))
        End While

        For Each pos As Integer In blankPositions
            chars(pos) = "_"c
        Next

        Return New String(chars)
    End Function

    ' Function to check the user's answer
    Private Sub CheckAnswer()
        Dim userAnswer As String = txtAnswer.Text

        If userAnswer.ToLower() = currentWord.ToLower() Then
            lblResult.Text = "Correct"
            lblResult.BackColor = Color.Green
            lblResult.ForeColor = Color.White
            score += 1
            PlaySound() ' Play sound on correct answer
        Else
            lblResult.Text = "Wrong"
            lblResult.BackColor = Color.Red
            lblResult.ForeColor = Color.White
            PlaySound() ' Play sound on wrong answer
        End If

        ' Show the correct word after checking the answer
        lblCorrectWord.Text = "Correct Word: " & currentWord

        Timer1.Interval = 1000
        Timer1.Start()
    End Sub

    ' Function to play sound
    Private Sub PlaySound()
        Dim soundFilePath As String = "game.wav"
        If System.IO.File.Exists(soundFilePath) Then
            Dim player As New SoundPlayer(soundFilePath)
            player.Play()
        End If
    End Sub

    ' Function to start the next round or finish the game
    Private Sub NextWord()
        currentRound += 1

        If currentRound <= rounds Then
            ' Pick a word from the list and partially blank it
            currentWord = GetRandomWord()
            lblWord.Text = PartiallyBlankWord(currentWord)

            txtAnswer.Clear()
            lblResult.Text = ""
            lblCorrectWord.Text = "" ' Clear the previous correct word
            lblResult.BackColor = Color.Transparent
        Else
            MessageBox.Show("Congratulations! You completed the game with a score of " & score & "/" & rounds & ".")
            Me.Close()
        End If
    End Sub

    ' Start the game
    Private Sub btnStart_Click(sender As Object, e As EventArgs)
        ' Ask user for difficulty level
        difficulty = InputBox("Choose difficulty: Easy, Medium, or Hard", "Select Difficulty", "Medium").Trim()

        ' Validate the difficulty level entered by the user
        If Not {"Easy", "Medium", "Hard"}.Contains(difficulty) Then
            MessageBox.Show("Invalid difficulty level. Please select Easy, Medium, or Hard.")
            Return
        End If

        currentRound = 1
        score = 0
        currentWord = GetRandomWord()
        lblWord.Text = PartiallyBlankWord(currentWord)
        txtAnswer.Clear()
        lblResult.Text = ""
        lblCorrectWord.Text = "" ' Clear the correct word at the start
        lblResult.BackColor = Color.Transparent
        btnStart.Enabled = False

        PlaySound() ' Play sound when starting the game
    End Sub

    ' Check the answer when the "Next" button is clicked
    Private Sub btnNext_Click(sender As Object, e As EventArgs)
        CheckAnswer()
    End Sub

    ' Move to the next word after showing the result
    Private Sub Timer1_Tick(sender As Object, e As EventArgs)
        Timer1.Stop()
        NextWord()
    End Sub
End Class
