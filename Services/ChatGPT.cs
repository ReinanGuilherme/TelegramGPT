using Newtonsoft.Json;
using System.Text;

namespace TelegramGPT.Services
{
    public class ChatGPT
    {
        private readonly IConfiguration _configuration;

        public ChatGPT(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        //Faz uma pergunta ao ChatGPT.
        public async Task<string> AskQuestions(string prompt)
        {
            try
            {
                //Recupera o token do ChatGPT a partir do arquivo appsettings.json e configura a variavel com essas informações.
                var apiKey = _configuration["ChatGPTSettings:Token"];

                var url = "https://api.openai.com/v1/completions";

                //Montando o objeto de requisição.
                var jsonRequest = JsonConvert.SerializeObject(new
                {
                    prompt = prompt, //prompt é a pergunta que será enviada ao ChatGPT
                    max_tokens = 2048,  //número máximo de tokens que serão retornados na resposta
                    n = 1, //número de respostas que serão retornadas
                    model = "text-davinci-003" //modelo do ChatGPT que será utilizado
                });

                //criando uma nova instância do HttpClient
                using (var client = new HttpClient())
                {
                    //adicionando o token de autorização na requisição
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
                    //criando o conteúdo da requisição
                    var httpContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                    //enviando a requisição
                    var response = await client.PostAsync(url, httpContent);

                    //verificando se a resposta foi um sucesso
                    if (!response.IsSuccessStatusCode)
                    {
                        //retorna uma string vazia caso a resposta não seja um sucesso
                        return string.Empty;
                    }
                    else
                    {
                        //recuperando o conteúdo da resposta
                        var jsonResponse = await response.Content.ReadAsStringAsync();
                        //convertendo o conteúdo da resposta para um objeto dinâmico
                        dynamic responseObject = JsonConvert.DeserializeObject(jsonResponse);
                        //retornando a primeira resposta
                        return responseObject.choices[0].text;
                    }
                }
            } catch(Exception ex)
            {
                //retorna uma string vazia caso ocorra algum erro
                return string.Empty;
            }
        }
    }
}
