'
'   Copyright 2011, 2012 Alec Skelly
'
'   This file is part of SANEWinDS.
'
'   SANEWinDS is free software: you can redistribute it and/or modify
'   it under the terms of the GNU General Public License as published by
'   the Free Software Foundation, either version 3 of the License, or
'   (at your option) any later version.
'
'   SANEWinDS is distributed in the hope that it will be useful,
'   but WITHOUT ANY WARRANTY; without even the implied warranty of
'   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
'   GNU General Public License for more details.
'
'   You should have received a copy of the GNU General Public License
'   along with SANEWinDS.  If not, see <http://www.gnu.org/licenses/>.
'
Public Class classUniversalConverter
    'Public Function TryParse(ByVal InValue As Object, ByVal DestinationType As Type, ByVal OutValue As Object) As Boolean
    '    Select Case DestinationType
    '        Case GetType(TWAIN_VB.TW_FIX32)
    '            Select Case InValue.GetType
    '                Case GetType(TWAIN_VB.TW_FIX32)
    '                    OutValue = InValue
    '                    Return True
    '                Case GetType(Single), GetType(Double)
    '                    OutValue = 


    '            End Select

    '    End Select
    'End Function

    'Public Function FloatToFIX32(ByVal floater As Double) As TW_FIX32
    '    Dim Fix32_value As TW_FIX32
    '    Dim sign As Boolean = (floater < 0)
    '    Dim value As Int32 = CType(floater * 65536.0 + IIf(sign, -0.5, 0.5), Int32)
    '    Fix32_value.Whole = CType(value >> 16, Int16)
    '    Fix32_value.Frac = CType(value And &HFFFFUS, UInt16)
    '    Return Fix32_value
    'End Function

    'Public Function FIX32ToFloat(ByVal _fix32 As TW_FIX32) As Double
    '    Return CType(_fix32.Whole, Double) + CType(_fix32.Frac / 65536.0, Double)
    'End Function
End Class
