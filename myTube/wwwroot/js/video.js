function setIgnorado(id, nome) {
    let params = {
        url: `@Url.Action("Assistido", "Video")/${id}`,
        method: 'get',
        success: () => {
            toastr.success('Video marcado como Assistido', `${nome}`);
            $(`#item-${id}`).fadeOut('slow', () => $(this).remove());
            $(`#item-${id}`).remove();
        },
        error: (resp) => { toastr.error(resp.statusText, `${nome}`); },
        complete: () => { }
    };

    $.ajax(params)
}

function setIgnorado(id, nome) {
    let params = {
        url: `@Url.Action("Ignorado", "Video")/${id}`,
        method: 'get',
        success: () => {
            toastr.warning('Video marcado como Ignorado', `${nome}`);
            $(`#item-${id}`).fadeOut('slow', () => $(this).remove());
            $(`#item-${id}`).remove();
        },
        error: (resp) => { toastr.error(resp.statusText, `${nome}`); },
        complete: () => { }
    };

    $.ajax(params)
}

function setFavorito(id, nome) {
    let params = {
        url: `@Url.Action("Favorito", "Video")/${id}`,
        method: 'get',
        success: () => {
            toastr.warning('Video marcado como Favorito', `${nome}`);
            $(`#item-${id}`).fadeOut('slow', () => $(this).remove());
            $(`#item-${id}`).remove();
        },
        error: (resp) => { toastr.error(resp.statusText, `${nome}`); },
        complete: () => { }
    };

    $.ajax(params)
}

function setAssistirDepois(id, nome) {
    let params = {
        url: `@Url.Action("AssistirDepois", "Video")/${id}`,
        method: 'get',
        success: () => {
            toastr.info('Video marcado como Assistir Depois', `${nome}`);
            $(`#item-${id}`).fadeOut('slow', () => $(this).remove());
            $(`#item-${id}`).remove();
        },
        error: (resp) => { toastr.error(resp.statusText, `${nome}`); },
        complete: () => { }
    };

    $.ajax(params)
}
function setConsole(id, nome) {
    console.log('Somente para teste')

    toastr.info('Video NÃO MARCADO', `${nome}`);
    $(`#item-${id}`).fadeOut('slow', () => $(this).remove());
    $(`#item-${id}`).remove();
}
