import { useState } from 'react'
import { api } from '../api.js'

const toForm = (m) => ({
  title: m.title ?? '',
  director: m.director ?? '',
  runtime: String(m.runtime ?? ''),
  releaseYear: String(m.releaseYear ?? ''),
  genre: m.genre ?? '',
  country: m.country ?? '',
  photoPath: m.photoPath ?? '',
  posterUrl: m.posterUrl ?? '',
  movieUrl: m.movieUrl ?? '',
  description: m.description ?? '',
})

export default function MovieEditForm({ movie, onSaved, onNotFound, onCancel }) {
  const [form, setForm] = useState(() => toForm(movie))
  const [fieldErrors, setFieldErrors] = useState({})
  const [message, setMessage] = useState(null)
  const [saving, setSaving] = useState(false)

  const change = (e) => setForm({ ...form, [e.target.name]: e.target.value })

  const fieldError = (name) => fieldErrors[name[0].toUpperCase() + name.slice(1)]?.join(' ')

  const submit = async (e) => {
    e.preventDefault()
    setSaving(true)
    setMessage(null)
    setFieldErrors({})
    try {
      await api.updateMovie(movie.id, { ...form, runtime: Number(form.runtime), releaseYear: Number(form.releaseYear) })
      onSaved()
    } catch (err) {
      if (err.status === 404) return onNotFound()
      if (err.fieldErrors) setFieldErrors(err.fieldErrors)
      else setMessage({ type: 'error', text: err.message })
    } finally {
      setSaving(false)
    }
  }

  const input = (name, props = {}) => (
    <label>
      {props.label}
      <input
        name={name}
        value={form[name]}
        onChange={change}
        className={fieldError(name) ? 'invalid' : undefined}
        {...props.input}
      />
      {fieldError(name) && <span className="field-error">{fieldError(name)}</span>}
    </label>
  )

  return (
    <form className="panel session-form" onSubmit={submit} noValidate>
      <h3>Edit film</h3>
      {input('title', { label: 'Title', input: { maxLength: 100 } })}
      {input('director', { label: 'Director', input: { maxLength: 100 } })}
      {input('runtime', { label: 'Runtime (min)', input: { type: 'number' } })}
      {input('releaseYear', { label: 'Release year', input: { type: 'number' } })}
      {input('genre', { label: 'Genre', input: { maxLength: 50 } })}
      {input('country', { label: 'Country', input: { maxLength: 50 } })}
      {input('photoPath', { label: 'Poster URL', input: { maxLength: 500 } })}
      {input('posterUrl', { label: 'Poster link', input: { maxLength: 500 } })}
      {input('movieUrl', { label: 'Movie link', input: { maxLength: 500 } })}
      <label className="wide">
        Description
        <textarea name="description" value={form.description} onChange={change} rows={4} maxLength={1000} />
        {fieldError('description') && <span className="field-error">{fieldError('description')}</span>}
      </label>
      {message && <p className="message error wide">{message.text}</p>}
      <div className="auth-buttons wide">
        <button type="submit" disabled={saving}>{saving ? 'Saving...' : 'Save'}</button>
        <button type="button" className="secondary" onClick={onCancel}>Cancel</button>
      </div>
    </form>
  )
}
