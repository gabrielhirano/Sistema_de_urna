using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
abstract class Urna
{
    static int numeroDeCandidatos = 5, op, cont = 0;
    static Candidato[] candidatos = new Candidato[numeroDeCandidatos];
    string cidadeUrna = "";

    public static void Menu()
    {
        Console.WriteLine("--------Menu--------");
        Console.WriteLine("1. Cadastrar eleição");
        Console.WriteLine("2. Adicionar Candidatos");
        Console.Write("\nOpção: ");
        op = int.Parse(Console.ReadLine());
        switch (op)
        {
            case 1:
                cadastrarEleicao();
                break;

            default:
                Console.WriteLine("Opção inválida.");
                break;
        }

    }

    public static void cadastrarEleicao()
    {
        Console.WriteLine("\n--------Tipo de eleição--------");
        Console.WriteLine("1. Legislativo");
        Console.WriteLine("2. Executivo");
        Console.Write("\nOpção: ");
        int op2 = int.Parse(Console.ReadLine());
        switch (op2)
        {
            case 1:
                registraVereador();
                break;
            case 2:
                // registraPrefeito();
                break;
            default:
                Console.WriteLine("Opção inválida.");
                break;
        }
    }

    public static List<string> listSet(List<string> lista){
        List<string> listaSemRedundancia = new List<string>();
        foreach(var item in lista.Distinct()){
            listaSemRedundancia.Add(item);
        }
        return listaSemRedundancia;
    }
  

    public static void registraVereador()
    {
        Hashtable listaPartidos = new Hashtable();
        Stream VencedorVereador = File.Open("files/VencedorVereador.txt", FileMode.Create);
        StreamWriter VencedorV = new StreamWriter(VencedorVereador);
        string nome, partido;
        int idade, numero, numeroDeVotos;
        Vereador[] arrayVereadores = new Vereador[35];
        string linha = "";
        Stream entrada = File.Open("files/candidatos.txt", FileMode.Open);
        StreamReader leitor = new StreamReader(entrada);
        linha = leitor.ReadLine();

        while (linha != null)
        {
            string[] candidato = linha.Split(';');
            nome = candidato[0];
            partido = candidato[1];
            idade = int.Parse(candidato[2]);
            numero = int.Parse(candidato[3]);
            numeroDeVotos = int.Parse(candidato[4]);
            arrayVereadores[cont] = new Vereador(candidato[0], candidato[1], int.Parse(candidato[2]), int.Parse(candidato[3]), int.Parse(candidato[4]));
            //Console.WriteLine(arrayVereadores[cont].numeroDeVotos);
            if(listaPartidos.ContainsKey(partido) == false){
                // Dictionary chave, valor  nomePartido e votos
                listaPartidos.Add(partido,numeroDeVotos);
            }else{
                listaPartidos[partido] = (int)listaPartidos[partido] + numeroDeVotos;
            }
            linha = leitor.ReadLine();
            cont++;
        }
        foreach (var item in listaPartidos.Keys){
            Console.WriteLine("Partido: " + item + " teve: " + listaPartidos[item] + " votos");
        }
        Console.WriteLine("\n\n");
        Hashtable vencedores = ResultadoVereador(arrayVereadores, listaPartidos);

        foreach (var vencedor in vencedores.Keys){
            Console.WriteLine("Partido: " + vencedor + " Ganhou: " + vencedores[vencedor] + " vaga(s)");
        }
        // {
        //     VencedorV.Write($"Vencedor {auxName}\t {candidato.getNome()}");
        //     linha = leitor.ReadLine();
        //     auxName++;
        // }

        //adicionar aqui o método específico para os vereadores!!!
        //VencedorV.Write("O vencedor entre os Vereadores é o(a): " + ResultadoVereador(arrayVereadores));
        entrada.Close();
        VencedorV.Close();
        VencedorVereador.Close();
        leitor.Close();
    }

    public static Candidato ResultadoPrefeito(Candidato[] candidatos, int totalDeVotosDaEleicao)
    {
        int i = 0, votos = 0;
        Candidato vencedor = new Candidato();
        for (i = 0; i < candidatos.Length; i++)
        {
            if (candidatos[i].numeroDeVotos > votos)
            {
                votos = candidatos[i].numeroDeVotos;
                vencedor = candidatos[i];
            }

            else if (candidatos[i].numeroDeVotos == vencedor.numeroDeVotos)
            {
                if (candidatos[i].idade > vencedor.idade)
                {
                    vencedor = candidatos[i];
                }

            }
        }
        return vencedor;
    }

    public static Hashtable ResultadoVereador(Vereador[] vereadores, Hashtable listaPartidos)
    {
        Hashtable vagasPorPartido = new Hashtable();
        double totalVagas = 6;
        double totalVotos = 0;
        double maiorMedia = 0;
        string mediaPartido = "";
        Vereador[] vencedores = new Vereador[(int)totalVagas];

        foreach(var item in listaPartidos.Values){
            totalVotos = totalVotos + (int)item;
        }

        double QElegislativo = totalVotos/totalVagas;
        double QPartidario;
        foreach(var item in listaPartidos.Keys){
            QPartidario = Math.Floor(Convert.ToDouble(listaPartidos[item])/QElegislativo);
            vagasPorPartido.Add(item,QPartidario);
            if(QPartidario != 0)
                totalVagas--;
        }
        Hashtable vagasppDesempate = new Hashtable();

        foreach(var item in vagasPorPartido.Keys){
            vagasppDesempate.Add(item,vagasPorPartido[item]);
        }
        while(totalVagas != 0){
            //remover os partidos com 0 votos e calcular media para repescagem de vagas
            foreach(var item in listaPartidos.Keys){
                if(Convert.ToDouble(vagasppDesempate[item]) == 0){
                    vagasppDesempate.Remove(item);
                }else{
                    vagasppDesempate[item] = Math.Floor(Convert.ToDouble(listaPartidos[item])/ (Convert.ToDouble(vagasPorPartido[item]) + 1));
                }
            }
            //Contabilizar partido que conquisatara a vaga
            foreach(var item in vagasppDesempate.Keys){
                if(maiorMedia < Convert.ToDouble(vagasppDesempate[item])){
                    maiorMedia = Convert.ToDouble(vagasppDesempate[item]);
                    mediaPartido = (string)item;
                }
            }
            maiorMedia = 0;
            vagasPorPartido[mediaPartido] = Convert.ToDouble(vagasPorPartido[mediaPartido]) + 1;
            totalVagas--;
        }
        return vagasPorPartido;
    }
}


