import { useEffect, useState } from 'react'

export function useLoad(load, deps) {
  const [state, setState] = useState({ data: null, loading: true, error: null, status: null })
  const [version, setVersion] = useState(0)

  useEffect(() => {
    let active = true
    setState((s) => ({ ...s, loading: true, error: null, status: null }))
    load()
      .then((data) => active && setState({ data, loading: false, error: null, status: null }))
      .catch((err) => active && setState({ data: null, loading: false, error: err.message, status: err.status ?? null }))
    return () => {
      active = false
    }
  }, [...deps, version])

  return { ...state, reload: () => setVersion((v) => v + 1) }
}
