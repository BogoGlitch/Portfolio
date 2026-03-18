type PageLayoutProps = {
  title: string;
  description?: string;
  undernav?: React.ReactNode;
  children?: React.ReactNode;
};

const PageLayout = ({ title, description, undernav, children }: PageLayoutProps) => {
  return (
    <main className="page-shell">
      <section className="page-header">
        {undernav ? <div className="undernav">{undernav}</div> : null}
        <h1>{title}</h1>
        {description ? <p>{description}</p> : null}
      </section>

      <section className="page-content">{children}</section>
    </main>
  );
};

export default PageLayout;
