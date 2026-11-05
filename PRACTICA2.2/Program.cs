using System.Globalization;
using System.Text;
using System.Xml;
 
string[] etiquetas = { "Title", "Genre", "Premiere", "Runtime", "IMDBScore", "Language" };
 
int opcionMenu = 0;
 
while (opcionMenu != 3)
{
    Console.WriteLine("\n--- MENU ---");
    Console.WriteLine("1. CSV a XML");
    Console.WriteLine("2. XML a CSV");
    Console.WriteLine("3. Salir");
    Console.Write("Opcion: ");
 
    if (!int.TryParse(Console.ReadLine(), out opcionMenu))
    {
        Console.WriteLine("Opcion incorrecta");
        continue;
    }
 
    try
    {
        if (opcionMenu == 1)
        {
            CSV_XML();
        }
        else if (opcionMenu == 2)
        {
            XML_CSV();
        }
        else if (opcionMenu != 3)
        {
            Console.WriteLine("Opcion incorrecta");
        }
    }
    catch (Exception e)
    {
        Console.WriteLine("Error: " + e.Message);
    }
}
 
void CSV_XML()
{
    Console.Write("Ruta del CSV: ");
    string rutaCSV = Console.ReadLine().Trim();
 
    if (!File.Exists(rutaCSV))
    {
        Console.WriteLine("El CSV no existe");
        return;
    }
 
    string[] lineas = File.ReadAllLines(rutaCSV);
    List<string[]> peliculas = new List<string[]>();
 
    for (int i = 1; i < lineas.Length; i++)
    {
        peliculas.Add(SepararLinea(lineas[i]));
    }
 
    peliculas = Filtrar(peliculas);
 
    Console.Write("Ruta para guardar el XML: ");
    string rutaXML = Console.ReadLine().Trim();
 
    string carpetaXML = Path.GetDirectoryName(rutaXML);
    if (carpetaXML != "")
        Directory.CreateDirectory(carpetaXML);
 
    XmlWriterSettings ajustes = new XmlWriterSettings();
    ajustes.Indent = true;
 
    XmlWriter xmlWriter = XmlWriter.Create(rutaXML, ajustes);
 
    xmlWriter.WriteStartDocument();
    xmlWriter.WriteStartElement("Peliculas");
 
    foreach (string[] p in peliculas)
    {
        xmlWriter.WriteStartElement("Pelicula");
 
        for (int i = 0; i < etiquetas.Length; i++)
        {
            xmlWriter.WriteStartElement(etiquetas[i]);
            xmlWriter.WriteString(p[i]);
            xmlWriter.WriteEndElement();
        }
 
        xmlWriter.WriteEndElement();
    }
 
    xmlWriter.WriteEndElement();
    xmlWriter.WriteEndDocument();
    xmlWriter.Close();
 
    Console.WriteLine("XML creado correctamente");
}
 
void XML_CSV()
{
    Console.Write("Ruta del XML: ");
    string rutaXML = Console.ReadLine().Trim();
 
    if (!File.Exists(rutaXML))
    {
        Console.WriteLine("El XML no existe");
        return;
    }
 
    XmlDocument xmlDoc = new XmlDocument();
    xmlDoc.Load(rutaXML);
 
    XmlNodeList peliculaNodes = xmlDoc.SelectNodes("/Peliculas/Pelicula");
    List<string[]> peliculas = new List<string[]>();
 
    foreach (XmlNode peliculaNode in peliculaNodes)
    {
        string[] p = new string[6];
 
        for (int i = 0; i < etiquetas.Length; i++)
        {
            XmlNode campo = peliculaNode.SelectSingleNode(etiquetas[i]);
            p[i] = campo.InnerText;
        }
 
        peliculas.Add(p);
    }
 
    peliculas = Filtrar(peliculas);
 
    Console.Write("Ruta para guardar el CSV: ");
    string rutaCSV = Console.ReadLine().Trim();
 
    StringBuilder sb = new StringBuilder();
    sb.AppendLine("Title,Genre,Premiere,Runtime,IMDB Score,Language");
 
    foreach (string[] p in peliculas)
    {
        string linea = "";
 
        for (int i = 0; i < p.Length; i++)
        {
            string campo = p[i];
 
            if (campo.Contains(","))
                campo = "\"" + campo + "\"";
 
            linea += campo;
 
            if (i < p.Length - 1)
                linea += ",";
        }
 
        sb.AppendLine(linea);
    }
 
    string carpetaCSV = Path.GetDirectoryName(rutaCSV);
    if (carpetaCSV != "")
        Directory.CreateDirectory(carpetaCSV);
 
    File.WriteAllText(rutaCSV, sb.ToString());
 
    Console.WriteLine("CSV creado correctamente");
}
 
List<string[]> Filtrar(List<string[]> peliculas)
{
    Console.WriteLine("\n1. Exportar todo");
    Console.WriteLine("2. Exportar por genero");
    Console.WriteLine("3. Exportar por año");
    Console.WriteLine("4. Exportar por idioma");
    Console.Write("Opcion: ");
 
    int opcion = Convert.ToInt32(Console.ReadLine());
 
    if (opcion == 2)
    {
        List<string> generos = new List<string>();
 
        foreach (string[] p in peliculas)
        {
            if (!generos.Contains(p[1]))
                generos.Add(p[1]);
        }
 
        foreach (string genero in generos)
            Console.WriteLine(genero);
 
        Console.Write("Genero: ");
        string generoElegido = Console.ReadLine();
 
        peliculas = peliculas.FindAll(p =>
            p[1].ToLower() == generoElegido.ToLower());
    }
    else if (opcion == 3)
    {
        Console.Write("Año: ");
        int año = Convert.ToInt32(Console.ReadLine());
 
        peliculas = peliculas.FindAll(p =>
            DateTime.Parse(p[2], new CultureInfo("en-US")).Year >= año);
    }
    else if (opcion == 4)
    {
        List<string> idiomas = new List<string>();
 
        foreach (string[] p in peliculas)
        {
            if (!idiomas.Contains(p[5]))
                idiomas.Add(p[5]);
        }
 
        foreach (string idioma in idiomas)
            Console.WriteLine(idioma);
 
        Console.Write("Idioma: ");
        string idiomaElegido = Console.ReadLine();
 
        peliculas = peliculas.FindAll(p =>
            p[5].ToLower() == idiomaElegido.ToLower());
    }
 
    return peliculas;
}
 
string[] SepararLinea(string linea)
{
    List<string> campos = new List<string>();
    string campo = "";
    bool entreComillas = false;
 
    foreach (char c in linea)
    {
        if (c == '"')
        {
            entreComillas = !entreComillas;
        }
        else if (c == ',' && !entreComillas)
        {
            campos.Add(campo);
            campo = "";
        }
        else
        {
            campo += c;
        }
    }
 
    campos.Add(campo);
    return campos.ToArray();
}