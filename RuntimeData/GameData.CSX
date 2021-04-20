
<Structures>
  <Structure Name="MMFGameData" AutoFill="0" AutoCreate="1" DefaultHex="0" AutoDestroy="0" DoNotSaveLocal="0" RLECompression="1" AutoCreateStructsize="4096">
    <Elements>
      <Element Offset="0" Vartype="String" Bytesize="4" OffsetHex="00000000" Description="head" DisplayMethod="Unsigned Integer"/>
      <Element Offset="4" Vartype="2 Bytes" Bytesize="2" OffsetHex="00000004" Description="runtime_version" DisplayMethod="Unsigned Integer"/>
      <Element Offset="6" Vartype="2 Bytes" Bytesize="2" OffsetHex="00000006" Description="runtime_subversion" DisplayMethod="Unsigned Integer"/>
      <Element Offset="8" Vartype="4 Bytes" Bytesize="4" OffsetHex="00000008" Description="product_version" DisplayMethod="Unsigned Integer"/>
      <Element Offset="12" Vartype="4 Bytes" Bytesize="4" OffsetHex="0000000C" Description="product_build" DisplayMethod="Unsigned Integer"/>
      <Element Offset="16" Vartype="4 Bytes" Bytesize="4" OffsetHex="00000010" Description="headerSize" DisplayMethod="Unsigned Integer"/>
      <Element Offset="20" Vartype="2 Bytes" Bytesize="2" OffsetHex="00000014" Description="headerFlags" DisplayMethod="Unsigned Integer"/>
      <Element Offset="22" Vartype="2 Bytes" Bytesize="2" OffsetHex="00000016" Description="headerNewFlags" DisplayMethod="Unsigned Integer"/>
      <Element Offset="24" Vartype="2 Bytes" Bytesize="2" OffsetHex="00000018" Description="headerGraphicsMode" DisplayMethod="Unsigned Integer"/>
      <Element Offset="26" Vartype="2 Bytes" Bytesize="2" OffsetHex="0000001A" Description="headerOtherFlags" DisplayMethod="Unsigned Integer"/>
      <Element Offset="28" Vartype="2 Bytes" Bytesize="2" OffsetHex="0000001C" Description="headerWindowWidth" DisplayMethod="Unsigned Integer"/>
      <Element Offset="30" Vartype="2 Bytes" Bytesize="2" OffsetHex="0000001E" Description="headerWindowHeight" DisplayMethod="Unsigned Integer"/>
      <Element Offset="32" Vartype="4 Bytes" Bytesize="4" OffsetHex="00000020" Description="headerScore" DisplayMethod="Unsigned Integer"/>
      <Element Offset="36" Vartype="4 Bytes" Bytesize="4" OffsetHex="00000024" Description="headerLives" DisplayMethod="Hexadecimal"/>
      <Element Offset="40" Vartype="Array of byte" Bytesize="72" OffsetHex="00000028" Description="Controls" DisplayMethod="Hexadecimal"/>
      <Element Offset="112" Vartype="4 Bytes" Bytesize="4" OffsetHex="00000070" Description="headerBorderColor" DisplayMethod="Hexadecimal"/>
      <Element Offset="116" Vartype="4 Bytes" Bytesize="4" OffsetHex="00000074" Description="headerNumberOfFrames" DisplayMethod="Unsigned Integer"/>
      <Element Offset="120" Vartype="4 Bytes" Bytesize="4" OffsetHex="00000078" Description="headerFrameRate" DisplayMethod="Unsigned Integer"/>
      <Element Offset="124" Vartype="4 Bytes" Bytesize="4" OffsetHex="0000007C" Description="headerWindowMenuIndex" DisplayMethod="Unsigned Integer"/>
      <Element Offset="128" Vartype="Pointer" Bytesize="4" OffsetHex="00000080" Description="AppNamePtr" DisplayMethod="Unsigned Integer">
        <Structure Name="Autocreated from 007C1CC8" AutoFill="0" AutoCreate="1" DefaultHex="0" AutoDestroy="0" DoNotSaveLocal="0" RLECompression="1" AutoCreateStructsize="4096">
          <Elements>
            <Element Offset="0" Vartype="Unicode String" Bytesize="500" OffsetHex="00000000" Description="AppName" DisplayMethod="Unsigned Integer"/>
          </Elements>
        </Structure>
      </Element>
      <Element Offset="132" Vartype="Pointer" Bytesize="4" OffsetHex="00000084" Description="ExecPathPtr" DisplayMethod="Unsigned Integer">
        <Structure Name="Autocreated from 007C8898" AutoFill="0" AutoCreate="1" DefaultHex="0" AutoDestroy="0" DoNotSaveLocal="0" RLECompression="1" AutoCreateStructsize="4096">
          <Elements>
            <Element Offset="0" Vartype="Unicode String" Bytesize="500" OffsetHex="00000000" Description="ExecPath" DisplayMethod="Unsigned Integer"/>
          </Elements>
        </Structure>
      </Element>
      <Element Offset="136" Vartype="Pointer" Bytesize="4" OffsetHex="00000088" Description="AppEditorFilenamePtr" DisplayMethod="Unsigned Integer">
        <Structure Name="Autocreated from 007C8898" AutoFill="0" AutoCreate="1" DefaultHex="0" AutoDestroy="0" DoNotSaveLocal="0" RLECompression="1" AutoCreateStructsize="4096">
          <Elements>
            <Element Offset="0" Vartype="Unicode String" Bytesize="500" OffsetHex="00000000" Description="AppEditorFilename" DisplayMethod="Unsigned Integer"/>
          </Elements>
        </Structure>
      </Element>
      <Element Offset="140" Vartype="Pointer" Bytesize="4" OffsetHex="0000008C" Description="CopyrightPtr" DisplayMethod="Unsigned Integer">
        <Structure Name="Autocreated from 007C8898" AutoFill="0" AutoCreate="1" DefaultHex="0" AutoDestroy="0" DoNotSaveLocal="0" RLECompression="1" AutoCreateStructsize="4096">
          <Elements>
            <Element Offset="0" Vartype="Unicode String" Bytesize="500" OffsetHex="00000000" Description="Copyright" DisplayMethod="Unsigned Integer"/>
          </Elements>
        </Structure>
      </Element>
      <Element Offset="144" Vartype="Pointer" Bytesize="4" OffsetHex="00000090" Description="AboutTextPtr" DisplayMethod="Unsigned Integer">
        <Structure Name="Autocreated from 007C8898" AutoFill="0" AutoCreate="1" DefaultHex="0" AutoDestroy="0" DoNotSaveLocal="0" RLECompression="1" AutoCreateStructsize="4096">
          <Elements>
            <Element Offset="0" Vartype="Unicode String" Bytesize="500" OffsetHex="00000000" Description="AboutText" DisplayMethod="Unsigned Integer"/>
          </Elements>
        </Structure>
      </Element>
      <Element Offset="148" Vartype="Pointer" Bytesize="4" OffsetHex="00000094" Description="AppTargetFilenamePtr" DisplayMethod="Unsigned Integer">
        <Structure Name="Autocreated from 007C8898" AutoFill="0" AutoCreate="1" DefaultHex="0" AutoDestroy="0" DoNotSaveLocal="0" RLECompression="1" AutoCreateStructsize="4096">
          <Elements>
            <Element Offset="0" Vartype="Unicode String" Bytesize="500" OffsetHex="00000000" Description="AppTargetFilename" DisplayMethod="Unsigned Integer"/>
          </Elements>
        </Structure>
      </Element>
      <Element Offset="152" Vartype="Pointer" Bytesize="4" OffsetHex="00000098" Description="UnkPtr" DisplayMethod="Unsigned Integer"/>
      <Element Offset="156" Vartype="4 Bytes" Bytesize="4" OffsetHex="0000009C" Description="fileHandle" DisplayMethod="Unsigned Integer"/>
    </Elements>
  </Structure>
</Structures>