export interface MvpPageAction {
  label: string
  icon: string
}

export interface MvpPageMetric {
  label: string
  value: string
  tone?: 'ready' | 'pending' | 'risk'
}

export interface MvpPageSection {
  title: string
  items: string[]
}

export interface MvpPageDefinition {
  sectionTitle: string
  title: string
  subtitle: string
  status: string
  source: string
  actions: MvpPageAction[]
  metrics: MvpPageMetric[]
  scope: MvpPageSection
  nextSteps: MvpPageSection
  risks: MvpPageSection
}
