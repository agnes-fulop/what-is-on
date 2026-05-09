import type { Session, Speaker } from './event';

export type ComponentType =
  | 'Section'
  | 'Heading'
  | 'Paragraph'
  | 'SpeakerList'
  | 'SpeakerCard'
  | 'SessionSchedule'
  | 'SessionCard';

export type HeadingLevel = 'h1' | 'h2' | 'h3' | 'h4' | 'h5' | 'h6';

export interface SectionData {
  direction: 'col' | 'row';
}

export interface HeadingData {
  text: string;
  level: HeadingLevel;
}

export interface ParagraphData {
  text: string;
}

export interface SpeakerListData {
  title: string;
}

export interface SpeakerCardData {
  speaker: Speaker | null;
}

export interface SessionScheduleData {
  title: string;
}

export interface SessionCardData {
  session: Session | null;
}

/**
 * Discriminated union on the parent's `type` field — narrowing on `type`
 * gives the matching `data` shape automatically. The renderer's switch
 * statement relies on this for exhaustiveness checks.
 */
export type LayoutComponent =
  | { id: string; type: 'Section'; data: SectionData; children: LayoutComponent[] }
  | { id: string; type: 'Heading'; data: HeadingData; children: LayoutComponent[] }
  | { id: string; type: 'Paragraph'; data: ParagraphData; children: LayoutComponent[] }
  | { id: string; type: 'SpeakerList'; data: SpeakerListData; children: LayoutComponent[] }
  | { id: string; type: 'SpeakerCard'; data: SpeakerCardData; children: LayoutComponent[] }
  | { id: string; type: 'SessionSchedule'; data: SessionScheduleData; children: LayoutComponent[] }
  | { id: string; type: 'SessionCard'; data: SessionCardData; children: LayoutComponent[] };

export interface LayoutDto {
  id: string;
  components: LayoutComponent[];
}
