using TelegramGPT.Services;

namespace TelegramGPT.EndPoints.WebHook
{
    public class WebHook
    {
        public static string Template => "/TelegramGPT/";
        public static string[] Methods => new string[] { HttpMethod.Post.ToString() };
        public static Delegate Handle => Action;

        // Adicionando o serviço do telegramBot criado no diretorio Services.
        public static async Task<IResult> Action(WebHookRequest request, ChatGPT chatGPT, TelegramBot tgBot)
        {
            try
            {
                var message = "Desculpe, mas não é possível processar sua mensagem pois ela não é uma mensagem de texto. Por favor, envie uma mensagem de texto válida.";
                if (!string.IsNullOrEmpty(request.Message.Text))
                {
                    //faz uma pergunta ao ChatGPT
                    message = await chatGPT.AskQuestions(request.Message.Text);
                }

                //recupera o id do chat
                var chatId = request.Message.Chat.Id;

                //envia a mensagem de resposta para o telegram
                var result = await tgBot.SendMessageTextAsync(chatId, message);

                if (!result)
                {
                    return Results.BadRequest();
                }

                return Results.Ok();
            } catch(Exception ex)
            {
                return Results.BadRequest();
            }
        }
    }
}
