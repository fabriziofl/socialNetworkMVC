using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Logica_De_Negocio
{
    public class Sistema
    {
        private List<Usuario> _usuarios = new List<Usuario>();
        private List<Publicacion> _publicaciones = new List<Publicacion>();
        private Random _random = new Random();
        private static Sistema _instancia;

        public Sistema() 
        {
            PrecargaMiembros();
            PrecargaAdmin();
            PrecargaSolicitudes();
            AgregarVinculos();
            PrecargaPublicaciones();
            PrecargaReacciones();
        }

        public static Sistema Instancia
        {
            get
            {
                if (_instancia == null) _instancia = new Sistema();
                return _instancia;
            }
        }

        public List<Usuario> Usuarios
        {
            get
            {
                return _usuarios;
            }
        }

        public void AltaMiembro(Miembro miembro)
        {
            miembro.ValidarDatos();

            if (!_usuarios.Contains(miembro))
            {
                _usuarios.Add(miembro);
            }
        }

        public void AltaAdmin(Administrador administrador)
        {
            administrador.ValidarDatos();

            if (!_usuarios.Contains(administrador))
            {
                _usuarios.Add(administrador);
            }
        }

        private void PrecargaMiembros()
        {
            for(int i = 0; i < 10; i++)
            {
                string email = "Miembro_" + i;
                string contrasenia = "Contra_" + i;
                string nombre = "Nombre_" + i;
                string apellido = "Apellido_" + i;
                DateTime fechaNac = DateTime.Now;

                Usuario miembroNuevo = new Miembro(email, contrasenia, nombre, apellido, fechaNac);
                AltaMiembro((Miembro)miembroNuevo);
            }


            Usuario miembroPrueba_1 = new Miembro("email_prueba", "contra_prueba", "Agustina", "Albear", DateTime.Now);
            Usuario miembroPrueba_2 = new Miembro("email_prueba_uno", "contra_prueba_uno", "Javier", "Hernandez", DateTime.Now);
            Usuario miembroPrueba_3 = new Miembro("email_prueba_dos", "contra_prueba_dos", "Alberto", "Hernandez", DateTime.Now);

            AltaMiembro((Miembro)miembroPrueba_1);
            AltaMiembro((Miembro)miembroPrueba_2);
            AltaMiembro((Miembro)miembroPrueba_3);
        }

        private void PrecargaAdmin()
        {
            Usuario adminNuevo = new Administrador("Admin", "Admin_1");

            AltaAdmin((Administrador)adminNuevo);
        }

        // Metodo para precarga de solicitudes. Para cada miembro de la lista le precarga una solicitud de amistad,
        // si la solicitud existe o la amistad existe la nueva solicitud NO se crea. A su vez si la solicitud se crea con el Estado Aprobado
        // se crea tambien la amistad reciproca.
        private void PrecargaSolicitudes()
        {
            for (int i = 0; i < _usuarios.Count; i++)
            {
                if (_usuarios[i] is Miembro)
                {
                    Miembro miembroSolicitante = (Miembro)_usuarios[i];
                    Miembro miembroSolicitado;

                    do
                    {
                        miembroSolicitado = ObtenerMiembroAleatorio();
                    } while (miembroSolicitante == miembroSolicitado);

                    

                    if(!miembroSolicitado.ExisteAmistad(miembroSolicitado) && !miembroSolicitado.ExisteSolicitud(miembroSolicitado))
                    {
                        EstadoDeSolicitud estadoRandom = GenerarEstadoRandom();

                        Solicitud nuevaSolicitud = new Solicitud(miembroSolicitante, miembroSolicitado, estadoRandom);

                        miembroSolicitado.AgregarSolicitud(nuevaSolicitud);

                        if (estadoRandom == EstadoDeSolicitud.APROBADA)
                        {
                            miembroSolicitado.AgregarAmigo(miembroSolicitante);
                            miembroSolicitante.AgregarAmigo(miembroSolicitado);

                        }
                    }
                }
            };
        }

        //Metodo para generar al menos dos vinculos de amistad de forma reciproca
        private void AgregarVinculos()
        {
            for (int i = 0; i < 2; i++)
            {
                Miembro miembroUno = ObtenerMiembroAleatorio();
                Miembro miembroDos;

                do
                {
                    miembroDos = ObtenerMiembroAleatorio();
                } while (miembroUno == miembroDos);

                miembroUno.AgregarAmigo(miembroDos);
                miembroDos.AgregarAmigo(miembroUno);
            }
        }

        //Precarga 15 Publicaciones y luego Precarga 3 Comentarios a cada una
        private void PrecargaPublicaciones()
        {
            for (int i = 0; i < 15; i++)
            {
                string titulo = "Titulo_" + i;
                string texto = "Publicacion_" + i;
                string imagen = i + "_imagen.jpg";
                Miembro miembro = ObtenerMiembroAleatorio();

                Publicacion nuevaPublicacion = new Post(titulo, texto, miembro, imagen);

                Post nuevoPost = (Post)nuevaPublicacion;
                
                nuevoPost.ValidarDatos();

                _publicaciones.Add(nuevaPublicacion);
                
                miembro.AgregarPublicacion(nuevaPublicacion);  
            }

            AgregarComentarios();
        }

        private void AgregarComentarios()
        {
            List<Publicacion> nuevosComentarios = new List<Publicacion>();

            foreach (Publicacion post in _publicaciones)
            {
                if(post is Post)
                {
                    for(int i = 0;i < 3; i++)
                    {
                        string titulo = "Titulo_Comentario_" + i;
                        string texto = "Comentario_" + i;
                        Post post2 = (Post)post;
                        Miembro miembro = ObtenerMiembroAleatorio();
                        

                        Publicacion nuevoComentario = new Comentario(titulo, texto, miembro, post2);

                        nuevoComentario.ValidarDatos();
                        nuevosComentarios.Add(nuevoComentario);

                        miembro.AgregarPublicacion(nuevoComentario);
                        post2.AgregarComentario((Comentario)nuevoComentario);
                    }
                }
            }

            _publicaciones.AddRange(nuevosComentarios);
        }

        //Precarga de Reacciones para Post y Comentarios
        private void PrecargaReacciones()
        {
            List<Post> posts = FiltrarPost();
            List<Comentario> comentarios = FiltrarComentarios();

            for (int i = 0; i < 2; i++)
            {
                int numRan = _random.Next(0, posts.Count);
                Reaccion nuevaReaccion = GenerarReaccion();

                posts[numRan].AgregarReaccion(nuevaReaccion);
            }

            for (int i = 0; i < 2; i++)
            {
                int numRan = _random.Next(0, comentarios.Count);
                Reaccion nuevaReaccion = GenerarReaccion();

                comentarios[numRan].AgregarReaccion(nuevaReaccion);
            }

        }

        private Miembro ObtenerMiembroAleatorio()
        {
            Miembro miembro = null;

            for (int i = 0; i < _usuarios.Count; i++)
            {
                int numRan = _random.Next(_usuarios.Count);

                if (_usuarios[numRan] is Miembro)
                {
                    miembro = (Miembro)_usuarios[numRan];
                    break; 
                }
            }

            return miembro;
        }

        private List<Post> FiltrarPost()
        {
            List<Post> postList = new List<Post>();

            foreach (Publicacion post in _publicaciones)
            {
                if(post is Post) { postList.Add((Post)post); }
            }

            return postList;
        }

        private List<Comentario> FiltrarComentarios()
        {
            List<Comentario> comentList = new List<Comentario>();

            foreach (Publicacion coment in _publicaciones)
            {
                if (coment is Comentario) { comentList.Add((Comentario)coment); }
            }

            return comentList;
        }

        private Reaccion GenerarReaccion()
        {
            bool meGusta = _random.Next(2) == 0;
            Miembro miembro = ObtenerMiembroAleatorio();

            Reaccion nuevaReaccion = new Reaccion(meGusta, miembro);

            return nuevaReaccion;
        }

        private EstadoDeSolicitud GenerarEstadoRandom()
        {
            int totalEstados = Enum.GetValues(typeof(EstadoDeSolicitud)).Length;

            EstadoDeSolicitud estadoRandom = (EstadoDeSolicitud)_random.Next(totalEstados);

            return estadoRandom;
        }

        public List<string> PublicacionesDeMiembro(string emailMiembro)
        {
            List<string> miembroLista = new List<string>();
            
            if(BuscarUsuario(emailMiembro) is Miembro && BuscarUsuario(emailMiembro) != null)
            {
                Miembro? miembro = (Miembro)BuscarUsuario(emailMiembro);

                foreach (Publicacion publicacion in miembro.Publicaciones)
                {
                    miembroLista.Add(publicacion.ToString());
                }
            }

            return miembroLista;
        }

        public List<string> PostComentadosPorMiembro(string emailMiembro)
        {
            List<string> listaPost = new List<string>();

            if (BuscarUsuario(emailMiembro) is Miembro miembro)
            {

                foreach (Publicacion publicacion in miembro.Publicaciones)
                {
                    if(publicacion is Comentario comentario)
                    {
                        bool existe = false;

                        foreach (string post in listaPost)
                        {
                            if (post == comentario.Post.ToString())
                            {
                                existe = true;
                            }
                        }

                        if (!existe)
                        {
                            listaPost.Add(comentario.Post.ToString());
                        }
                    }
                }
            }
            return listaPost;
        }

        public List<Publicacion> DevolverPostEntreFechas(DateTime fechaInicial, DateTime fechaFinal)
        {
            List<Publicacion> listaDePost = new List<Publicacion>();

            foreach (Publicacion publicacion in _publicaciones)
            {
                if(publicacion is Post)
                {
                    if(fechaInicial <= publicacion.DateTime && fechaFinal >= publicacion.DateTime)
                    {
                        listaDePost.Add(publicacion);
                    }
                }
            }
            listaDePost.Sort();

            return listaDePost;
        }

        public List<Miembro> MiembroConMasPublicaciones()
        {
            List<Miembro> listDeMiembrosConMasPublicaciones = new List<Miembro>();

            foreach (Usuario usuario in _usuarios)
            {
                if(usuario is Miembro miembro)
                {
                    if (listDeMiembrosConMasPublicaciones.Count == 0)
                    {
                        listDeMiembrosConMasPublicaciones.Add(miembro);
                    } else
                    {
                        if (listDeMiembrosConMasPublicaciones[0].Publicaciones.Count() == miembro.Publicaciones.Count())
                        {

                            listDeMiembrosConMasPublicaciones.Add(miembro);
                        }

                        if (listDeMiembrosConMasPublicaciones[0].Publicaciones.Count() < miembro.Publicaciones.Count())
                        {
                            listDeMiembrosConMasPublicaciones.Clear();

                            listDeMiembrosConMasPublicaciones.Add(miembro);
                        }
                    }
                }
            }
            return listDeMiembrosConMasPublicaciones;
        }

        public bool UsuarioExiste(string emailMiembro)
        {
            bool existe = false;

            if (BuscarUsuario(emailMiembro) != null)
            {
                existe = true;
            }

            return existe;
        }

        public Usuario? BuscarUsuario(string emailMiembro)
        {
            Usuario? usuarioBuscado = null;    

            foreach (Usuario usuario in _usuarios)
            {
                if (usuario.Email == emailMiembro)
                {
                    usuarioBuscado = usuario;
                }
            }

            return usuarioBuscado;
        }

        public Publicacion BuscarPost(int id)
        {
            Publicacion postBuscado = null;

            foreach (Publicacion publicacion in _publicaciones)
            {
                if (publicacion.Id == id)
                {
                    postBuscado = publicacion;
                }
            }

            return postBuscado;
        }

        public List<Miembro> DevolverMiembros()
        {
            List<Miembro> listaMiembros = new List<Miembro>();

            foreach (Usuario usuario in _usuarios)
            {
                if(usuario is Miembro)
                {
                    listaMiembros.Add((Miembro)usuario);
                }
            }

            listaMiembros.Sort();

            return listaMiembros;
        }

        public List<Post> DevolverPostParaUsuario(string emailUsuario)
        {
            List <Post> posts = new List<Post>();

            foreach (Publicacion publicacion in _publicaciones)
            {
                if(publicacion is Post post)
                {
                    posts.Add(post);
                }
                
            }

            return posts;
        }
        public List<Post> DevolverPost()
        {
            List<Post> posts = new List<Post>();

            foreach (Publicacion publicacion in _publicaciones)
            {
                if (publicacion is Post post)
                {
                    posts.Add(post);
                }

            }

            return posts;
        }
    }
}
