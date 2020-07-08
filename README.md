# E-Players
## Projeto de aprendizagem MVC com ASP Net Core

### Parte 1 - Models
> Criamos nossos models Equipe, Jogador e Partida com suas devidas propriedades
```c#
    public class Equipe
    {
        public int IdEquipe { get; set; }
        public string Nome { get; set; }
        public string Imagem { get; set; }
    }
```

```c#
    public class Jogador
    {
        public int IdJogador { get; set; }
        public string Nome { get; set; }
        public int IdEquipe { get; set; }
    }
```

```c#
    public class Partida
    {
        public int IdPartida { get; set; }
        public int IdJogador1 { get; set; }
        public int IdJogador2 { get; set; }
        public DateTime HorarioInicio { get; set; }
        public DateTime HorarioTermino { get; set; }
    }
```
> Criamos uma classe chamada EPlayersBase para abstrair alguns métodos de manipulação do CSV
```c#
    public class EplayersBase
    {     
        public void CreateFolderAndFile(string _path){

            string folder   = _path.Split("/")[0];
            string file     = _path.Split("/")[1];

            if(!Directory.Exists(folder)){
                Directory.CreateDirectory(folder);
            }

            if(!File.Exists(_path)){
                File.Create(_path).Close();
            }
        }
    }
```
> Também criamos uma interface para Equipe para solidificar nossa estrutura:
```c#
using E_Players.Models;

namespace E_Players.Interfaces
{
    public interface IEquipe
    {
        void Create(Equipe e);

        List<Equipe> ReadAll();

        void Update(Equipe e);

        void Delete(int id);
    }
}
```

> Com a interface criada, herdamos em Equipe a classe EplayersBase e a interface IEquipe
```c#
public class Equipe : EplayersBase , IEquipe
```
> Implantamos a interface e começamos o desenvolvimento do método Create:
```c#
using System;
using System.IO;
using E_Players.Interfaces;

namespace E_Players.Models
{
    public class Equipe : EplayersBase , IEquipe
    {
        public int IdEquipe { get; set; }
        public string Nome { get; set; }
        public string Imagem { get; set; }
        private const string PATH = "Database/equipe.csv";

        public Equipe(){
            CreateFolderAndFile(PATH);
        }

        public void Create(Equipe e){
            string[] linha = { Prepare(e) };
            File.AppendAllLines(PATH, linha);
        }

        private string Prepare(Equipe e){
            return $"{e.IdEquipe};{e.Nome};{e.Imagem}";
        }

        public List<Equipe> ReadAll()
        {
            throw new NotImplementedException();
        }

        public void Update(Equipe e)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}
```
> Depois, dentro de Equipe, criamos as funcionalidades do método ReadAll
```c#
        public List<Equipe> ReadAll()
        {
            List<Equipe> equipes = new List<Equipe>();
            string[] linhas = File.ReadAllLines(PATH);
            foreach (var item in linhas)
            {
                string[] linha = item.Split(";");
                Equipe equipe = new Equipe();
                equipe.IdEquipe = Int32.Parse(linha[0]);
                equipe.Nome = linha[1];
                equipe.Imagem = linha[2];

                equipes.Add(equipe);
            }
            return equipes;
        }
```
<br><br>

> Dentro de EplayersBase, criamos o método que vai nos ajudar a ler todas as linhas de um csv, e o outro para re-escrevê-lo. Lembre-se de que eles poderão ser aplicados em qualquer classe que herdr de EplayersBase:
```c#
        public List<string> ReadAllLinesCSV(string PATH){
            
            List<string> linhas = new List<string>();
            using(StreamReader file = new StreamReader(PATH))
            {
                string linha;
                while((linha = file.ReadLine()) != null)
                {
                    linhas.Add(linha);
                }
            }
            return linhas;
        }

        public void RewriteCSV(string PATH, List<string> linhas)
        {
            using(StreamWriter output = new StreamWriter(PATH))
            {
                foreach (var item in linhas)
                {
                    output.Write(item + "\n");
                }
            }
        }
```
> Com esses métodos base criados, nossos métodos restantes, Update e Delete ficam mais fáceis de serem implantados:
```c#
        public void Update(Equipe e)
        {
            List<string> linhas = ReadAllLinesCSV(PATH);
            linhas.RemoveAll(x => x.Split(";")[0] == e.IdEquipe.ToString());
            linhas.Add( Prepare(e) );
            RewriteCSV(PATH, linhas);
        }

        public void Delete(Equipe e)
        {
            List<string> linhas = ReadAllLinesCSV(PATH);
            linhas.RemoveAll(x => x.Split(";")[0] == e.IdEquipe.ToString());
            RewriteCSV(PATH, linhas);
        }
```

### Parte 2 - Controllers

> Criamos a classe ***EquipeController***
```c#
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using E_Players.Models;

namespace E_Players.Controllers
{
    public class EquipeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
```
> Dentro da classe instanciamos nosso model de Equipe e passamos ele como retorno da View, para conseguir acessá-lo via Razor
```c#
        Equipe equipeModel = new Equipe();

        public IActionResult Index()
        {
            ViewBag.Equipes = equipeModel.ReadAll();
            return View();
        }
```

> Criamos o método *Cadastrar()*, passando como argumento um *IFormCollection*:
```c#
    public IActionResult Cadastrar(IFormCollection form)
    {
        
    }
```
> Depois capturamos as informações que virão do formulário e passamos para um objeto do tipo **Equipe**
```c#
        public IActionResult Cadastrar(IFormCollection form)
        {
            Equipe novaEquipe   = new Equipe();
            novaEquipe.IdEquipe = Int32.Parse(form["IdEquipe"]);
            novaEquipe.Nome     = form["Nome"];
            novaEquipe.Imagem   = form["Imagem"];

            equipeModel.Create(novaEquipe);            
            ViewBag.Equipes = equipeModel.ReadAll();

            return LocalRedirect("~/Equipe");
        }
```

### Parte 3 - Views

> Dentro da pasta Views, criamos um diretório chamado Equipe, e dentro dele um arquivo chamado Index.cshtml <br>
> Dentro deste arquivo chamamos nossa model e mudamos a ViewData do Title
```c#
    @model Equipe
    @{
        ViewData["Title"] = "Equipes";
    }
```
> Logo em baixo criamos um form bem simples para testar, implementando a ação via Razor:
```c#

<form method="POST" action='@Url.Action("Cadastrar")'>
    <label>ID</label>
    <input type="text" name="IdEquipe" />

    <label>Nome</label>
    <input type="text" name="Nome" />

    <label>Imagem</label>
    <input type="text" name="Imagem" />

    <button type="submit">Cadastrar</button>
</form>

<table class="table table-striped table-responsive">
    <thead>
        <th>ID</th>
        <th>Nome</th>
        <th>Foto</th>
    </thead>
    <tbody>
        @foreach(Equipe e in ViewBag.Equipes){
            <tr>
                <td>@e.IdEquipe</td>
                <td>@e.Nome</td>
                <td>@e.Imagem</td>
            </tr>
        }
    </tbody>
</table>
```
> Dentro da pasta Views/Home/Indesx.cshtml, adicionamos um link para abrir nossa página:
```c#
<a class="navbar-brand" asp-area="" asp-controller="Equipe" asp-action="Index">Equipes</a>
```
> Rodamos e testamos nossa aplicação com:
```bash
dotnet run
```

### Parte 4 - Upload de Imagem
> No método Cadastrar colocamos todo o comportamento necessário para realizar o upload de imagem:
```c#
        public IActionResult Cadastrar(IFormCollection form)
        {
            Equipe novaEquipe   = new Equipe();
            novaEquipe.IdEquipe = Int32.Parse(form["IdEquipe"]);
            novaEquipe.Nome     = form["Nome"];

            // Upload Início
            var file    = form.Files[0];
            var folder  = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/Equipes");

            if(file != null)
            {
                if(!Directory.Exists(folder)){
                    Directory.CreateDirectory(folder);
                }

                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/", folder, file.FileName);
                using (var stream = new FileStream(path, FileMode.Create))  
                {  
                    file.CopyTo(stream);  
                }
                novaEquipe.Imagem   = file.FileName;
            }
            else
            {
                novaEquipe.Imagem   = "padrao.png";
            }
            // Upload Final

            equipeModel.Create(novaEquipe);
            ViewBag.Equipes = equipeModel.ReadAll();

            return LocalRedirect("~/Equipe");
        }
```
> Adicionamos o ***enctype*** para permitir a inserção de arquivos e mudamos o tipo do input *Imagem* para ***file***:
```c#
<form method="POST" action='@Url.Action("Cadastrar")' enctype="multipart/form-data">

    <label>ID</label>
    <input type="text" name="IdEquipe" />

    <label>Nome</label>
    <input type="text" name="Nome" />

    <label>Imagem</label>
    <input type="file" name="Imagem" />

    <button type="submit">Cadastrar</button>

</form>
```

### Parte 5 - Excluir
> Em EquipeController adicionamos o metodo Excluir
```c#
        [Route("{id}")]
        public IActionResult Excluir(int id)
        {
            equipeModel.Delete(id);
            ViewBag.Equipes = equipeModel.ReadAll();
            return LocalRedirect("~/Equipe");
        }

```
> E na tabela do Index.cshtml adicionamos mais uma coluna com o link para excluir:
```c#
<table class="table table-striped table-responsive">
    <thead>
        <th>ID</th>
        <th>Nome</th>
        <th>Foto</th>
    </thead>
    <tbody>
        @foreach(Equipe e in ViewBag.Equipes){
            <tr>
                <td>@e.IdEquipe</td>
                <td>@e.Nome</td>
                <td><img src="img/Equipes/@e.Imagem" alt="Imagem da equipe @e.Nome" width="40"></td>
                <td><a asp-area="" asp-controller="Equipe" asp-action="Excluir" asp-route-id="@e.IdEquipe">Excluir</a></td>
            </tr>
        }
    </tbody>
</table>
```

### Parte 6 - Aplicando o design base
> Colocamos dentro da raiz nosso diretório base do e-players<br>
> Copiamos as pastas ***css*** e ***images*** do **E-PlayersBase** para a pasta ***wwwroot***<br>
> Dentro de Views > Shared > _Layout.cshtml, importamos o css e removemos o que vem por padrão no projeto, alterando também o lang para pt-br:
```html
<!DOCTYPE html>
<html lang="pt-br">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>@ViewData["Title"] - E_Players</title>
    <!-- Novo CSS e JS -->
    <link href="https://fonts.googleapis.com/css?family=Titillium+Web:400,700&display=swap" rel="stylesheet">
    <script src="https://kit.fontawesome.com/2faae0d38b.js" crossorigin="anonymous"></script>
    <link rel="stylesheet" href="~/css/style.css" />

</head>
```

> Depois trocamos para nosso Header da base e atualizamos os links para asp net:
```html
    <header>
        <div class="container cabecalho">
            <div>

                <a href="#" onclick="botaoMenu()">
                    <i class="fas fa-bars"></i>
                </a>

                <a asp-area="" asp-controller="Home" asp-action="Index">
                <img src="Images/Logo.svg" alt="Logo do site E-Players">
            </a>
            </div>
            <nav id="menu">
                <ul>
                    <li><a asp-area="" asp-controller="Home" asp-action="Index" title="voltar para a página inicial">INÍCIO</a></li>
                    <li><a href="#horarios" title="Ir até a sessão de horários das partidas">HORÁRIOS</a></li>
                    <li><a href="#resultados" title="Ir até a sessão de resultados das partidas">RESULTADOS</a></li>
                    <li><a href="#noticias" title="Ir até a sessão de notícias">NOTÍCIAS</a></li>
                    <li><a href="#" title="buscar conteudo no site"><i class="fas fa-search"></i></a></li>
                </ul>
            </nav>
        </div>
    </header>
```

> Fazemos o mesmo com o footer, removendo as bibliotecas de jquery desnecessárias nesse projeto:
```html
    <footer>
        <div class="container">
            <div>
                <h5>INSTITUCIONAL</h5>
                <nav>
                    <ul>
                        <li><a href="#">Fale Conosco</a></li>
                        <li><a href="#">Regras</a></li>
                        <li><a href="#">Suporte</a></li>
                        <li><a href="#">Política de Privacidade</a></li>
                        <li><a href="#">Termos de Uso</a></li>
                        <li><a href="#">Anuncie</a></li>
                    </ul>
                </nav>
            </div>
            <div class="mob_logo_sociais">
                <img src="Images/Logo.svg" alt="Logo do projeto E-Players">
                <div class="sociais">
                    <i class="fab fa-facebook-square"></i>
                    <i class="fab fa-instagram-square"></i>
                    <i class="fab fa-twitter-square"></i>
                    <i class="fab fa-youtube-square"></i>
                </div>
            </div>
        <div> 
            <h5>PRINCIPAIS LIGAS</h5>
            <h6>LIGA CBL</h6>
            <h6>LIGA LCK</h6>
            <h6>LIGA PROFISSIONAL</h6>
            <h6>LIGA WCG</h6>
        </div>
    </div>
        <p>© Copyrigth 2077 - Todos os direitos reservados ao sena-code</p>
    </footer>

    <script src="~/js/site.js" asp-append-version="true"></script>
```

> Depois vamos para a pasta Home > Index e adicionamos nosso main lá dentro:

```html
@{
    ViewData["Title"] = "Home Page";
}

<div class="banner">
    <a asp-area="" asp-controller="Jogador" asp-action="Index" class="btn gradient">INSCREVA-SE</a>
</div>

<div class="sobre">
    <div class="container">
        <div>
            <h1>O QUE É MUNDO EM COMUM ?</h1>
            <p> Lorem ipsum dolor sit amet consectetur adipisicing elit. Officiis corporis voluptas odio
                asperiores officia, quisquam eius optio itaque nobis vel. Amet placeat veniam architecto magni
                at facere eaque adipisci laborum. Lorem ipsum dolor sit amet consectetur adipisicing elit. At
                ipsum, veniam ad, illum corrupti nisi fugiat repellat harum aspernatur cupiditate aperiam
                voluptatibus nulla a excepturi facere id non nemo totam! Lorem ipsum dolor sit amet consectetur
                adipisicing elit. Cum numquam expedita architecto quia debitis sint consequatur! Deleniti sed
                itaque illum aut officiis, quasi tempora esse id incidunt consequatur, magni ipsa? Lorem ipsum
                dolor, sit amet consectetur adipisicing elit. Natus, beatae ullam iure blanditiis dolorum illum
                excepturi tempore a unde, ipsa facilis aut et tempora nam quaerat nemo saepe, minus eaque. Lorem
                ipsum dolor sit amet consectetur adipisicing elit. Tempore ab recusandae, consectetur dicta
                molestias aliquid eligendi laboriosam sint optio dolor, accusantium quasi eius ad, placeat atque
                harum veniam fuga sequi. Lorem ipsum dolor sit amet consectetur adipisicing elit. Eveniet </p>
        </div>
    </div>
</div>
<img src="Images/9a6e4f50b0d94f75ce274348f06a6d56.png" alt="Imagem de um 
campeão do lol" class="champion">

<section id="horarios">

    <div class="container">
        <div class="ao_vivo">
            <h2>AO VIVO</h2>
            <div class="partida">
                <strong>SK</strong>
                <img src="Images/SK.png" alt="Logo da equipe SK">
                <I>VS</I>
                <img src="Images/FLA.png" alt="Logo da equipe do Flamengo">
                <strong>FLA</strong>
            </div>
        </div>

        <div class="em_breve">
            <h2>EM BREVE</h2>
            <div class="partida">
                <strong>SK</strong>
                <img src="Images/SK.png" alt="Logo da equipe SK">
                <I>VS</I>
                <img src="Images/FLA.png" alt="Logo da equipe do Flamengo">
                <strong>FLA</strong>
            </div>
            <div class="partida">
                <strong>SK</strong>
                <img src="Images/SK.png" alt="Logo da equipe SK">
                <I>VS</I>
                <img src="Images/FLA.png" alt="Logo da equipe do Flamengo">
                <strong>FLA</strong>
            </div>
            <div class="partida">
                <strong>SK</strong>
                <img src="Images/SK.png" alt="Logo da equipe SK">
                <I>VS</I>
                <img src="Images/FLA.png" alt="Logo da equipe do Flamengo">
                <strong>FLA</strong>
            </div>
        </div>
    </div>
</section>

<div class="container content">
    <section class="noticias">
        <div class="card">
            <img src="Images/Teams/T1.jpeg" alt="Notícia sobra assunto 1">
            <div>
            <h4>Título da Noticia</h4>
            <p> Lorem, ipsum dolor sit amet consectetur adipisicing elit. Quos quibusdam quidem praesentium
                velit vel sapiente nemo obcaecati veritatis dolorum odio facere, quisquam mollitia quo eum.
                Aspernatur quam reiciendis ratione dolorem!</p>
                </div>
        </div>


        <div class="card">
            <img src="Images/Teams/T2.jpg" alt="Notícia sobra assunto 1">
            <div>
            <h4>Título da Noticia</h4>
            <p> Lorem, ipsum dolor sit amet consectetur adipisicing elit. Quos quibusdam quidem praesentium
                velit vel sapiente nemo obcaecati veritatis dolorum odio facere, quisquam mollitia quo eum.
                Aspernatur quam reiciendis ratione dolorem!</p>
                </div>
        </div>

        <div class="card">
            <img src="Images/Teams/T3.jpeg" alt="Notícia sobra assunto 1">
            <div>
            <h4>Título da Noticia</h4>
            <p> Lorem, ipsum dolor sit amet consectetur adipisicing elit. Quos quibusdam quidem praesentium
                velit vel sapiente nemo obcaecati veritatis dolorum odio facere, quisquam mollitia quo eum.
                Aspernatur quam reiciendis ratione dolorem!</p>
        </div>
        </div>

        <div class="card">
            <img src="Images/Teams/T9.jpeg" alt="Notícia sobra assunto 1">
            <div>
            <h4>Título da Noticia</h4>
            <p> Lorem, ipsum dolor sit amet consectetur adipisicing elit. Quos quibusdam quidem praesentium
                velit vel sapiente nemo obcaecati veritatis dolorum odio facere, quisquam mollitia quo eum.
                Aspernatur quam reiciendis ratione dolorem!</p>
                </div>
        </div>

        <div class="card">
            <img src="Images/Teams/T5.jpeg" alt="Notícia sobra assunto 1">
            <div>
            <h4>Título da Noticia</h4>
            <p> Lorem, ipsum dolor sit amet consectetur adipisicing elit. Quos quibusdam quidem praesentium
                velit vel sapiente nemo obcaecati veritatis dolorum odio facere, quisquam mollitia quo eum.
                Aspernatur quam reiciendis ratione dolorem!</p>
                </div>
        </div>

        <div class="card">
            <img src="Images/Teams/T7.jpeg" alt="Notícia sobra assunto 1">
            <div>
            <h4>Título da Noticia</h4>
            <p> Lorem, ipsum dolor sit amet consectetur adipisicing elit. Quos quibusdam quidem praesentium
                velit vel sapiente nemo obcaecati veritatis dolorum odio facere, quisquam mollitia quo eum.
                Aspernatur quam reiciendis ratione dolorem!</p>
                </div>
        </div>
    </section>
    <section class="recentes" id="resultados">
        <h3>PARTIDAS RECENTES</h3>

        <div class="resultado">
            <div class="team">
                <strong>SK</strong>
                <img src="Images/SK.png" alt="Logo da equipe SK">
                <I>VS</I>
                <img src="Images/FLA.png" alt="Logo da equipe do Flamengo">
                <strong>FLA</strong>
            </div>
            <div class="placar">
                <P>3 x 2</p>
            </div>
        </div>
        <div class="resultado">
            <div class="team">
                <strong>SK</strong>
                <img src="Images/SK.png" alt="Logo da equipe SK">
                <I>VS</I>
                <img src="Images/FLA.png" alt="Logo da equipe do Flamengo">
                <strong>FLA</strong>
            </div>
            <div class="placar">
                <P>3 x 2</p>
            </div>
            </div>
            <div class="resultado">
                <div class="team">
                    <strong>SK</strong>
                    <img src="Images/SK.png" alt="Logo da equipe SK">
                    <I>VS</I>
                    <img src="Images/FLA.png" alt="Logo da equipe do Flamengo">
                    <strong>FLA</strong>
                </div>
                <div class="placar">
                    <P>3 x 2</p>
                </div>
                </div>
                <div class="resultado">
                    <div class="team">
                        <strong>SK</strong>
                        <img src="Images/SK.png" alt="Logo da equipe SK">
                        <I>VS</I>
                        <img src="Images/FLA.png" alt="Logo da equipe do Flamengo">
                        <strong>FLA</strong>
                    </div>
                </div>
                    <div class="placar">
                        <P>3 x 2</p>
                    </div>
                    <div class="resultado">
                        <div class="team">
                            <strong>SK</strong>
                            <img src="Images/SK.png" alt="Logo da equipe SK">
                            <I>VS</I>
                            <img src="Images/FLA.png" alt="Logo da equipe do Flamengo">
                            <strong>FLA</strong>
                        </div>
                    </div>
                        <div class="placar">
                            <P>3 x 2</p>
                        </div>
                        <div class="resultado">
                            <div class="team">
                                <strong>SK</strong>
                                <img src="Images/SK.png" alt="Logo da equipe SK">
                                <I>VS</I>
                                <img src="Images/FLA.png" alt="Logo da equipe do Flamengo">
                                <strong>FLA</strong>
                            </div>
                            <div class="placar">
                                <P>3 x 2</p>
                            </div>

                        </div>
                        <a href="todos_os_resultados.html" class="btn gradient">mais resultados</a>
    </section>
</div>

```

> Na pasta JS, no arquivo site.js, colocamos nosso script que faz o menu funcionar corretamente no modo responsivo:
```js
// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function botaoMenu(){

    var m = document.getElementById("menu")
    if(m.style.display == "block"){
        m.style.display = "none";
    }else{
        m.style.display = "block"
    }
}
```

> Tiramos a div .container que envolvia o main, pra não prejudicar nosso css, ficando assim:
```html
    </header>

    <main role="main" class="pb-3">
        @RenderBody()
    </main>

    <footer>
```

> Testamos nossa aplicação:
```bash
dotnet run
```

> Em ***Equipe > Index.cshtml*** Ajustamos nosso form para ficar com a aparência definida no *style.css*:
```html
<div class="titulo_pagina">
    <h1>cadastro de equipe</h1>
</div>

<form method="POST" action='@Url.Action("Cadastrar")' enctype="multipart/form-data" class="cadastro">

    <div class="campo">
        <label for="IdEquipe">ID</label>
        <input type="text" name="IdEquipe" id="IdEquipe" />
    </div>

    <div class="campo">            
        <label for="Nome">Nome</label>
        <input type="text" name="Nome" id="Nome" />
    </div>

    <div class="campo">
        <label for="Imagem">Imagem</label>
        <input type="file" name="Imagem" for="Imagem" />
    </div>

    <button class="gradient btn" type="submit">Cadastrar</button>

</form>
```


