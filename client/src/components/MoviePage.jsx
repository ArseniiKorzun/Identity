import { useState } from 'react'
import { api } from '../api.js'
import { useLoad } from '../useLoad.js'
import { TIME_ZONE_NOTE } from '../format.js'
import SessionList from './SessionList.jsx'
import MovieEditForm from './MovieEditForm.jsx'

export default function MoviePage({ movieId }) {
  const movie = useLoad(() => api.getMovie(movieId), [movieId])
  const sessions = useLoad(() => api.getSessions({ movieId }), [movieId])
  const [editing, setEditing] = useState(false)
  const [notice, setNotice] = useState(null)

  const saved = () => {
    setEditing(false)
    setNotice({ type: 'success', text: 'Changes saved.' })
    movie.reload()
  }

  const notFound = () => {
    setEditing(false)
    movie.reload()
  }

  if (movie.loading && !movie.data) return <p>Loading...</p>
  if (movie.status === 404) {
    return (
      <section className="movie-page">
        <a className="back-link" href="#/movies">← All films</a>
        <p className="message error">Фільм не знайдено</p>
      </section>
    )
  }
  if (movie.error) return <p className="message error">{movie.error}</p>

  const { title, director, runtime, releaseYear, genre, country, description } = movie.data

  return (
    <section className="movie-page">
      <a className="back-link" href="#/movies">← All films</a>

      <div className="page-toolbar">
        <div className="movie-hero">
          <h2>{title}</h2>
          <p className="movie-meta">
            by {director} · {runtime} min{releaseYear > 0 && ` · ${releaseYear}`}
            {genre && ` · ${genre}`}{country && ` · ${country}`}
          </p>
        </div>
        {!editing && <button className="secondary" onClick={() => { setNotice(null); setEditing(true) }}>Edit</button>}
      </div>

      {notice && <p className={`message ${notice.type}`}>{notice.text}</p>}
      {editing
        ? <MovieEditForm movie={movie.data} onSaved={saved} onNotFound={notFound} onCancel={() => setEditing(false)} />
        : description && <p className="movie-description">{description}</p>}

      <h3>Upcoming sessions</h3>
      <p className="hint">{TIME_ZONE_NOTE} Sorted from nearest.</p>
      {sessions.error && <p className="message error">{sessions.error}</p>}
      {sessions.loading && !sessions.data
        ? <p>Loading sessions...</p>
        : sessions.data && (sessions.data.length
          ? <SessionList sessions={sessions.data} showMovie={false} />
          : <p className="empty">No upcoming sessions for this film.</p>)}
    </section>
  )
}
