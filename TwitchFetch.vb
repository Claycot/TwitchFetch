Public Class TwitchFetch
    'Code by claycot

    'Declaring vars.
    Dim rawinput As String
    Dim channelname As String
    Dim randomnum As Integer
    Dim tokenraw As String
    Dim rawlength As Integer
    Dim token As String
    Dim sig As String
    Dim m3uraw As String
    Dim outputsource As String
    Dim outputhigh As String
    Dim outputmedium As String
    Dim outputlow As String
    Dim outputmobile As String

    'Function to retrieve text from input url.
    Public Function LoadSiteContent(ByVal url As String) As String
        'create a new WebClient object
        Dim client As New System.Net.WebClient()
        'create a byte array for holding the returned data
        Dim html As Byte() = client.DownloadData(url)
        'use the UTF8Encoding object to convert the byte
        'array into a string
        Dim utf As New System.Text.UTF8Encoding()
        'return the converted string
        Return utf.GetString(html)
    End Function

    'Large button.
    Private Sub btnfetch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnfetch.Click
        rawinput = txtinput.Text
        channelname = rawinput.Trim.ToLower
        randomnum = CInt(Math.Floor((999999 - 1 + 1) * Rnd())) + 1
        tokenraw = LoadSiteContent("http://api.twitch.tv/api/channels/" & channelname & "/access_token")

        'Takes tokenraw and extracts token and sig.
        For x As Integer = 0 To tokenraw.Length - 3 Step 1
            If tokenraw.Substring(x, 3) = "sig" Then
                token = tokenraw.Substring(10, x - 13).Replace("\", "")
                sig = tokenraw.Substring(x + 6, 40)
            End If
        Next

        'Grabs m3u8 as string using the required variables.
        m3uraw = LoadSiteContent("http://usher.twitch.tv/api/channel/hls/" & channelname & ".m3u8?player=twitchweb&token=" & token & "&sig=" & sig & "&$allow_audio_only=true&allow_source=true&type=any&p=" & randomnum)

        'Parses m3uraw for all occurrances of "http://".
        Dim m3uparse As New System.Text.RegularExpressions.Regex("http://.*")
        Dim m3umatch As System.Text.RegularExpressions.MatchCollection = m3uparse.Matches(m3uraw)

        'For each parsed link, a variable is assigned based on quality.
        For Each link As System.Text.RegularExpressions.Match In m3umatch
            If link.Value.Contains("chunked") Then
                outputsource = link.Value
            ElseIf link.Value.Contains("high") Then
                outputhigh = link.Value
            ElseIf link.Value.Contains("medium") Then
                outputmedium = link.Value
            ElseIf link.Value.Contains("low") Then
                outputlow = link.Value
            ElseIf link.Value.Contains("mobile") Then
                outputmobile = link.Value
            End If
        Next

        'Depending on what quality is checked, the link is outputted into the rtf box.
        If rdqualsrc.Checked = True Then
            rtfout.Text = outputsource
        ElseIf rdqualhigh.Checked = True Then
            rtfout.Text = outputhigh
        ElseIf rdqualmed.Checked = True Then
            rtfout.Text = outputmedium
        ElseIf rdquallow.Checked = True Then
            rtfout.Text = outputlow
        Else
            rtfout.Text = "Please select a quality option."
        End If
    End Sub
End Class
